using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using OMS.Data.Entites.Accounting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Service.ServicesJWT
{
    public  interface ITokenService
    {
        public Task<string> CreateToken(AppUser user, UserManager<AppUser> userManager, IConfiguration _Configuration);
    }
}
