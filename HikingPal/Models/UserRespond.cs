using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HikingPal.Models
{
    public class UserRespond
    {
        public Guid UserID { get; set; }
        
        public string FirstName { get; set; }
                
        public string LastName { get; set; }

        public string Email { get; set; }

        public Role Role { get; set; }
    }
}
