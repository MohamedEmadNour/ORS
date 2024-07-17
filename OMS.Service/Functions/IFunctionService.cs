using OMS.Data.Entites.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Service.Functions
{
    public interface IFunctionService
    {
        Task AddFunctionsAsync(IEnumerable<tbFunctions> functions);
        Task<IEnumerable<tbFunctions>> GetAllFunctionsAsync();
        Task AddFunctionRolesAsync(IEnumerable<tbFunctionRoles> functionRoles);

        Task UpdateFunctionsAsync(IEnumerable<tbFunctions> functions);

    }
}
