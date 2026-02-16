using System.ComponentModel.DataAnnotations;

namespace RestroManagementSystem.Models
{
    public class Dish
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public string? Unit { get; set; }

        public string? Course { get; set; }

        public bool? IsAvailable { get; set; }

        public virtual ICollection<Orderdish>? Orderdishes { get; set; } = new List<Orderdish>();

    }
}
