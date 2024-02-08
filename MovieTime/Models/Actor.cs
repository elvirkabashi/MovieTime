using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieTime.Models
{
    public class Actor
    {
        public int ActorId { get; set; }
        public string Name { get; set; }
        [DataType(DataType.Date)]
        public DateTime Birthdate { get; set; }
        public string? Img { get; set; }
        public string Description {  get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public List<MovieActor>? MovieActor { get; set; }
    }
}
