using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HikingPal.Models
{
    public class Hike
    {
        [Key]
        public Guid HikeID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid AuthorID { get; set; }
        public User? Author { get; set; }
        public string? PhotoUrl { get; set; }
        public string? PhotoTitle { get; set;}
        [JsonIgnore]
        public ICollection<HikeUser>? HikeUsers { get; set; }
       
    }
}