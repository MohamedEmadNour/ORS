using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using OMS.Service.UserServ;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;

namespace OMS.Service.Functions
{
    public class DynamicFunctionAuthorizeAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        private readonly string _functionName;

        public DynamicFunctionAuthorizeAttribute(string functionName)
        {
            _functionName = functionName;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();

            var user = context.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var userIdentity = user.Identity as ClaimsIdentity;
            string userId = userIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var hasAccess = await userService.UserHasAccessAsync(userId, _functionName);
            if (!hasAccess)
            {
                context.Result = new UnauthorizedResult();
            }
        }

    }
}
