using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HikingPal.Models
{
    public class HikeResponse
    {        
        public Guid HikeID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }        
        public UserRespond Author { get; set; }
        public string? PhotoUrl { get; set; }
        public string? PhotoTitle { get; set; }
        public List<UserRespond> Hikers { get; set; }
    }
}
