using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RestroManagementSystem.Models
{
    public class Orderdish
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int DishId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPriceAtOrder { get; set; }

        public decimal Subtotal { get; set; }

        public decimal ItemDiscount { get; set; }

        public decimal ItemTotal { get; set; }

        [JsonIgnore]
        public virtual Dish? Dish { get; set; } 

        [JsonIgnore]
        public virtual Order? Order { get; set; }
    }
}
