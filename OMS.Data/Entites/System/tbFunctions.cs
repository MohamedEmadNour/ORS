using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Data.Entites.System
{
    public class tbFunctions : BaseEntity<long>
    {
        public string FunctionName { get; set; }
        public virtual ICollection<tbFunctionRoles> tbFunctionRoles { get; set; }

        public bool IsDelete { get; set; } = false;

    }
}
