using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Repositores.DTO
{
    public class FunctionDisplayDTO
    {
        public int FunctionId { get; set; }
        public string FunctionName { get; set; }
        public bool IsAdminFunction { get; set; }
        public bool IsUserFunction { get; set; }
    }
}
