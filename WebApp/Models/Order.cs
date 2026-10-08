using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class Order
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Vui long nhap ten khach hang")]
        public string CustomerName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vui long nhap SDT")]
        public string Phone { get; set; } = string.Empty;
        [Required(ErrorMessage = "Vui long nhap dia chi")]
        public string Address { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Pending";

        public decimal TotalAmount { get; set; }

        public List<OrderDetail> OrderDetails { get; set; } = new();
    }
}