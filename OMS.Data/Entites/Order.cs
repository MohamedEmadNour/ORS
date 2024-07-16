
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
        public decimal TotalAmount { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public PaymentMethods PaymentMethod { get; set; }
        public string Status { get; set; }

        public Customer Customer { get; set; }
    }
}
