using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HikingPal.Models
{
    public class Hike : HikeDTO
    {
        [Required]
        public Guid AuthorID { get; set; }
        
        public User? Author { get; set; }        
        [JsonIgnore]
        public ICollection<HikeUser>? HikeUsers { get; set; }       
    }
}