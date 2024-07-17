using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Repositores.DTO
{
    public class EmailDTO
    {
        public List<string> ProductName { get; set; } = new List<string>();
        public decimal TotalAmount { get; set; }
        public string Body { get; set; }

    }
}
