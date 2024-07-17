using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMS.Data.DBCOntext.Identity;
using iText.Commons.Actions.Contexts;
using Microsoft.AspNetCore.Identity;
using OMS.Data.Entites.Accounting;
using OMS.Data.Entites.Const;

namespace OMS.Service.UserServ
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppIdentityDbContext _context;

        public UserService(UserManager<AppUser> userManager, AppIdentityDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<bool> UserHasAccessAsync(string userId, string functionName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            if (await _userManager.IsInRoleAsync(user, RolesConstants.SuperAdmin))
            {
                return true;
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var function = await _context.tbFunctions
                                           .Include(f => f.tbFunctionRoles)
                                           .FirstOrDefaultAsync(f => f.FunctionName == functionName);

            if (function == null) return false;

            foreach (var role in userRoles)
            {
                if (function.tbFunctionRoles.Any(fr => fr.RoleId == role))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
