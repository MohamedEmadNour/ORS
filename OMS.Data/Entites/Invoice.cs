using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Data.Entites
{
    public class Invoice : BaseEntity<int>
    {
        public int InvoiceId { get; set; }
        public int OrderId { get; set; }
        public DateTime InvoiceDate { get; set; } = DateTime.Now;
        public decimal TotalAmount { get; set; }
        public Order Order { get; set; }
    }
}
