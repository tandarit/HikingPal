
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HikingPal.Models
{
    public class User : UserCreateRequest
    {
        [JsonIgnore]
        public Guid UserID { get; set; }
        [JsonIgnore]
        public string Salt { get; set; }

        [JsonIgnore]
        public int Iteration { get; set; }

        [JsonIgnore]
        public ICollection<HikeUser>? HikeUsers { get; set; }

        [JsonIgnore]
        public ICollection<Hike>? HikeAutors { get; set; }

    }
}
