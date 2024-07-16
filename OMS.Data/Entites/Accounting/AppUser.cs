using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Data.Entites.Accounting
{
    public class AppUser : IdentityUser
    {

        public ICollection<IdentityUserRole<string>> UserRoles { get; set; }
    }
}
