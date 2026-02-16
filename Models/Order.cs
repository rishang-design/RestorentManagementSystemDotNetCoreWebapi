using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RestroManagementSystem.Models
{
    
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal FinalAmount { get; set; }

        public string? Status { get; set; }

        public virtual ICollection<Orderdish> Orderdishes { get; set; } = new List<Orderdish>();

        [JsonIgnore]
        public virtual User? User { get; set; }
    }
}
