using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Data.Entites.System
{
    public class tbFunctions : BaseEntity<long>
    {
        public int tbFunctionsId { get; set; }
        public string FunctionName { get; set; }
        public bool IsAdminFunction { get; set; }
        public bool IsUserFunction { get; set; }
        public virtual ICollection<tbFunctionRoles> tbFunctionRoles { get; set; } = new List<tbFunctionRoles>();

        public bool IsDelete { get; set; } = false;

    }
}
