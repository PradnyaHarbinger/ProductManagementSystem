using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProductManagementSystem.Models
{
    public class ProductModel
    {
        [DisplayName("Id")]
        [Key]
        public Guid ProdId { get; set; }

        [Required]
        [DisplayName("Name")]
        public string Name { get; set; }
        [Required]

        [DisplayName("Description")]
        public string Description { get; set; }

        [Required]

        [DisplayName("Category")]
        public string Category { get; set; }

        [Required]

        [DisplayName("Price")]
        public double Price { get; set; }

    }
}
