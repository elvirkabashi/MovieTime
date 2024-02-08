using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieTime.dto;
using MovieTime.Models;
using System.Threading.Tasks;

namespace MovieTime.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            //_roleManager = roleManager;
        }

        public IActionResult GetUsers(string sortOrder, string[] roles, int page = 1)
        {
            const int pageSize = 10;

            ViewData["StatusSortParm"] = string.IsNullOrEmpty(sortOrder) ? "status_desc" : "status_asc";
            ViewData["UserNameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "username_asc" : "username_desc";

            var users = _userManager.Users.ToList();

            if (roles != null && roles.Length > 0)
            {
                // Filter users based on selected roles
                users = users.Where(u => roles.All(r => _userManager.IsInRoleAsync(u, r).Result)).ToList();
            }

            switch (sortOrder)
            {
                case "username_asc":
                    users = users.OrderBy(u => u.UserName).ToList();
                    break;
                case "username_desc":
                    users = users.OrderByDescending(u => u.UserName).ToList();
                    break;
                case "status_desc":
                    users = users.OrderByDescending(u => u.IsActivated).ToList();
                    break;
                case "status_asc":
                    users = users.OrderBy(u => u.IsActivated).ToList();
                    break;
                default:
                    break;
            }

            var totalItems = users.Count;
            var pagination = new Pagination(totalItems, page, pageSize);
            users = users
                .Skip((pagination.CurrentPage - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToList();

            ViewBag.Pagination = pagination;

            // Pass selected roles back to the view to maintain state
            ViewBag.SelectedRoles = roles;

            return View(users);
        }



        public async Task<IActionResult> ChangeUserRole(string userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var existingRoles = await _userManager.GetRolesAsync(user);

            if (newRole == "Admin")
            {

                await _userManager.AddToRoleAsync(user, "Admin");

                await _userManager.AddToRoleAsync(user, "User");
            }
            else if (newRole == "User")
            {
                await _userManager.RemoveFromRoleAsync(user, "Admin");
            }

            return RedirectToAction("GetUsers");
        }


        [HttpPost]
        public async Task<IActionResult> ChangeUserRolePost(string userId, string newRole)
        {
            await ChangeUserRole(userId, newRole);
            return RedirectToAction("GetUsers");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("GetUsers");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }


                return View("GetUsers", _userManager.Users);
            }
        }

        public async Task<IActionResult> ActivateUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            user.IsActivated = true;
            await _userManager.UpdateAsync(user);

            return RedirectToAction("GetUsers");
        }

        public async Task<IActionResult> DeactivateUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            user.IsActivated = false;
            await _userManager.UpdateAsync(user);

            return RedirectToAction("GetUsers");
        }
    }
}
