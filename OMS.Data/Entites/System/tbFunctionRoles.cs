using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Data.Entites.System
{
    public class tbFunctionRoles : BaseEntity<long>
    {
        public virtual tbFunctions tbFunctions { get; set; }
        public string RoleId { get; set; }
        public virtual AppRole AppRole { get; set; }

        public bool IsDelete { get; set; } = false;

    }
}
