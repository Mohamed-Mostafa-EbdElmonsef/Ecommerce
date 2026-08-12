using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Models
{
    [PrimaryKey(nameof(ProductId),nameof(Color))]
    public class ProductColor
    {
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }
        public string Color { get; set; }
    }
}
