using Microsoft.EntityFrameworkCore;
using OMS.Data.Entites.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMS.Data.DBCOntext.Identity;

namespace OMS.Service.Functions
{
    public class FunctionService : IFunctionService
    {
        private readonly AppIdentityDbContext _dbContext;

        public FunctionService(AppIdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddFunctionsAsync(IEnumerable<tbFunctions> functions)
        {
            _dbContext.tbFunctions.AddRange(functions);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<IEnumerable<tbFunctions>> GetAllFunctionsAsync()
        {
            return await _dbContext.tbFunctions.Where(delete => delete.IsDelete == false).ToListAsync();
        }
        public async Task AddFunctionRolesAsync(IEnumerable<tbFunctionRoles> functionRoles)
        {
            _dbContext.tbFunctionRoles.AddRange(functionRoles);
            await _dbContext.SaveChangesAsync();
        }
    }
}
