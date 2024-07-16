using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMS.Data.DBCOntext.Identity;

namespace OMS.Service.UserServ
{
    public class UserService : IUserService
    {
        private readonly AppIdentityDbContext _dbContext;

        public UserService(AppIdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> UserHasAccessAsync(string userId, string functionName)
        {
            var hasAccess = await (from func in _dbContext.tbFunctions
                                   join funcRole in _dbContext.tbFunctionRoles on func.Id equals funcRole.Id
                                   join userRole in _dbContext.UserRoles on funcRole.RoleId equals userRole.RoleId
                                   where userRole.UserId == userId && func.FunctionName == functionName && func.IsDelete == false && funcRole.IsDelete == false
                                   select func).AnyAsync();

            return hasAccess;
        }
    }
}
