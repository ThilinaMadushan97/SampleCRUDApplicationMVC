using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CRUDApplicationMVC.Models
{
    public class CreateProduct
    {
     
        [Required,MaxLength(100)]
        public string Name { get; set; } = "";
        [Required,MaxLength(100)]
        public string Brand { get; set; } = "";
        [Required,MaxLength(100)]
        public string Category { get; set; } = "";
  
        public decimal Price { get; set; }
        [Required]
        public string Description { get; set; } = "";
   
        public IFormFile? ImageFileName { get; set; }
    }
}
