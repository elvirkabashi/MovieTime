using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MovieTime.Configuration;
using MovieTime.dto;
using MovieTime.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MovieTime.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
       // private readonly JwtConfig _jwtConfig;

        public AuthenticationController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
           // _jwtConfig = jwtConfig;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistretionRequestDto requestDto)
        {
            if (ModelState.IsValid)
            {
                var user_exist = await _userManager.FindByEmailAsync(requestDto.Email);
                var name_exist = await _userManager.FindByNameAsync(requestDto.Name);

                if (user_exist != null)
                {
                    return BadRequest(new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>()
                        {
                            "Email already exist"
                        }
                    });
                }
                if (name_exist != null)
                {
                    return BadRequest(new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>()
                        {
                            "Name already exist"
                        }
                    });
                }

                //create user
                var new_user = new ApplicationUser()
                {
                    FullName = requestDto.FullName,
                    Email = requestDto.Email,
                    UserName = requestDto.Name,
                    IsActivated = true
                };

                var is_created = await _userManager.CreateAsync(new_user, requestDto.Password);

                if (is_created.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                        await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

                    if (await _roleManager.RoleExistsAsync(UserRoles.User))
                    {
                        await _userManager.AddToRoleAsync(new_user, UserRoles.User);
                    }
                    //var token = GenerateJwtToken(new_user);

                    return Ok(new AuthResult()
                    {
                        Result = true  
                    });
                }

                return BadRequest(new AuthResult()
                {
                    Errors = new List<string>()
                        {
                            "Server error"
                        },
                    Result = false
                });
            }

            return BadRequest();
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDto loginRequest)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(loginRequest.Email);

                if (existingUser == null)
                {
                    return BadRequest(new AuthResult()
                    {
                        Errors = new List<string>() { "Invalid payload" },
                        Result = false
                    });
                }

                if (!existingUser.IsActivated)
                {
                    return BadRequest(new AuthResult()
                    {
                        Errors = new List<string>() { "Account is deactivated" },
                        Result = false
                    });
                }

                var isCorrect = await _userManager.CheckPasswordAsync(existingUser, loginRequest.Password);

                if (!isCorrect)
                {
                    return BadRequest(new AuthResult()
                    {
                        Errors = new List<string>() { "Invalid Credentials" },
                        Result = false
                    });
                }

                var jwtToken = GenerateJwtToken(existingUser);

                return Ok(new AuthResult()
                {
                    Token = jwtToken,
                    Result = true
                });
            }

            return BadRequest(new AuthResult()
            {
                Errors = new List<string>() { "Invalid payload" },
                Result = false
            });
        }


        private string GenerateJwtToken(ApplicationUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Secret").Value);

            //token description
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new []
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString()),
                }),

                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            // Add user roles to claims
            var userRoles = _userManager.GetRolesAsync(user).Result;
            foreach (var role in userRoles)
            {
                tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            return jwtTokenHandler.WriteToken(token);
        }


    }


}
