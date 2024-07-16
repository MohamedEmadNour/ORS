using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Repositores.DTO
{
    public class OrderItemViewModel : OrderItemDTO
    {
        public int OrderItemId { get; set; }

        public string ProductName { get; set; }


        public decimal Discount { get; set; }
    }

    public class OrderItemDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
