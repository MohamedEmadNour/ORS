using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Data.Entites
{
    public class Customer : BaseEntity<int>
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public List<Order> Orders { get; set; }
    }
}
