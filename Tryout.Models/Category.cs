using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tryout.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [DisplayName("Category Name")]
        public string Name { get; set; }
        [DisplayName("Category Description")]

        public string Description { get; set; }
    }
}
