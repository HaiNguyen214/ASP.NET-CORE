using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Vui long nhap san pham")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vui long ghi mo ta")]
        public string Description { get; set; } = string.Empty;
        [Range(0, double.MaxValue, ErrorMessage = "Gia phai lon hon khong")]
        public decimal Price { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng không hợp lệ")]
        public int Quantity { get; set; }

        public string ImgURL { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vui long chon danh muc")]
        public int CategoryId { get; set; }

        public Category Category { get; set; } = null!;

        public bool Status { get; set; }
    }
}