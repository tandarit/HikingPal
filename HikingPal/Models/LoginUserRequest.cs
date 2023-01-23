using System.ComponentModel.DataAnnotations;

namespace HikingPal.Models
{
    public class LoginUserRequest
    {
        [Required(ErrorMessage = "Please enter LoginName")]
        [MinLength(10)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter Password")]
        [MinLength(12)]
        public string Password { get; set; }

    }
}
