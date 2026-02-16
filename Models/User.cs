using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RestroManagementSystem.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Address { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public string Password { get; set; }

        [JsonIgnore]
        public virtual ICollection<Order>? Orders { get; set; } = new List<Order>();

    }
}
