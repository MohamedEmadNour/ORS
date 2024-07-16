
using OMS.Data.Entites.Const;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Repositores.DTO
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public PaymentMethods PaymentMethod { get; set; }

        public List<OrderItemDTO> OrderItems { get; set; }

    }

    public class OrderDTO : OrderViewModel
    {

        public int CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
  
    }
    public class CreateOrderDTO : OrderViewModel
    {

    }

    public class UpdateOrderStatusDTO
    {
        public string Status { get; set; }
    }


}
