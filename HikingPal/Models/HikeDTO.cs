using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HikingPal.Models
{
    public class HikeDTO
    {
        [Key]
        public Guid HikeID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [NotMapped]
        public UserDTO? AuthorDTO { get; set; }
        public string? PhotoUrl { get; set; }
        public string? PhotoTitle { get; set; }
        [NotMapped]
        public List<UserDTO>? HikersDTO { get; set; }
    }
}
