using System.ComponentModel.DataAnnotations;
namespace WebApp.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Vui long nhap danh muc")]

        public string Name { get; set; } = string.Empty;
    }
}