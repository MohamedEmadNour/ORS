
using OMS.Data.Entites.Const;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Repositores.DTO
{
    public class OrderViewModel : CreateOrderDTO
    {
        public int OrderId { get; set; }

    }

    public class OrderDTO : OrderViewModel
    {

        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }


    }
    public class CreateOrderDTO 
    {
        public DateTime OrderDate { get; set; }
        public PaymentMethods PaymentMethod { get; set; }
        [Required]
        public List<OrderItemDTO> OrderItems { get; set; } = new List<OrderItemDTO>();

        public int CustomerId { get; set; }

        public string Email { get; set; }
    }

    public class UpdateOrderStatusDTO
    {
        [Required]
        public OrderStatus Status { get; set; }
    }


}
