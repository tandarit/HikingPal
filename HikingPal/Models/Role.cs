using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HikingPal.Models
{
    public class Role
    {        
        [Key]
        public Guid RoleID { get; set; }
        public string RoleName { get; set; }

        public string RoleDescription { get; set; }

        [JsonIgnore]
        public ICollection<User> Users { get; set; }


    }
}
