
using OMS.Data.Entites.Const;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Data.Entites
{

    public class Order : BaseEntity<int>
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public PaymentMethods PaymentMethod { get; set; }
        public List<OrderItem> OrderItems { get; set; }

        public Customer Customer { get; set; }
    }
}
