using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HikingPal.Models
{
    public class UserCreateRequest: UserDTO
    {       

        [Required(ErrorMessage = "Please enter Password")]
        [MinLength(12)]
        public string Password { get; set; }
    }
}
