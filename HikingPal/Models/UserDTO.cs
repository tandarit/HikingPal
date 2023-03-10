using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HikingPal.Models
{
    public class UserDTO
    {
        [Key]
        public Guid? UserID { get; set; }

        [Required(ErrorMessage = "Please enter LoginName")]
        [MinLength(10)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter Email ID")]
        [EmailAddress]
        public string Email { get; set; }
        
        public Guid? RoleID { get; set; }
        
        public Role? Role { get; set; }
    }
}
