using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HikingPal.Models
{
    public class UserCreateRequest: LoginUserRequest
    {
        [Required(ErrorMessage = "Please enter Email ID")]
        [EmailAddress]
        public string Email { get; set; }
                
        [Required(ErrorMessage = "Please enter First Name")]         
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter Last Name")]
        public string LastName { get; set; }

        
        [JsonIgnore]
        public Guid RoleID { get; set; }

        [JsonIgnore]
        public Role? Role { get; set; }
    }
}
