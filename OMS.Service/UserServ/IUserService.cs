using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Service.UserServ
{
    public interface IUserService
    {
        Task<bool> UserHasAccessAsync(string userId, string functionName);
    }
}
