using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HikingPal.Models
{
    public class HikeUser
    {
        [JsonIgnore]
        public Guid HikeUserID { get; set; }
        
        public Guid UserID { get; set; }
        [JsonIgnore]
        public User? User { get; set; }
        
        public Guid HikeID { get; set; }

        [JsonIgnore]
        public Hike? Hike { get; set; }
    }
}
