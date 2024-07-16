using Microsoft.AspNetCore.Identity;
using OMS.Data.Entites.Accounting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Data.Entites.System
{
    public class AppRole : IdentityRole
    {
        public ICollection<IdentityUserRole<string>> UserRoles { get; set; }
        public virtual ICollection<tbFunctionRoles> tbFunctionRoles { get; set; }

    }
}
