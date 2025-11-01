using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tryout.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public string SKU { get; set; }
        [Required]
        public string InspirationBrand { get; set; }
        [Required]
        [Display(Name = "Price (6 ml)")]
        [Range(1, 1000)]
        public double Price6ml { get; set; }

        [Required]
        [Display(Name = "Price (10 ml)")]
        [Range(1, 1000)]
        public double Price10ml { get; set; }

        [Required]
        [Display(Name = "Price (15 ml)")]
        [Range(1, 1000)]
        public double Price15ml { get; set; }

        [Required]
        [Display(Name = "Price (20 ml)")]
        [Range(1, 1000)]
        public double Price20ml { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category Category { get; set; }
        [ValidateNever]
        public List<ProductImage> ProductImages { get; set; }
    }
}
