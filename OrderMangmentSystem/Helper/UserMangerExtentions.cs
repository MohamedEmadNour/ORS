using Microsoft.AspNetCore.Identity;
using OMS.Data.Entites.Accounting;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace OrderMangmentSystem.Helper
{
    public static class UserMangerExtentions
    {
        public static async Task<AppUser?> FineUserWithAddressAsync(this UserManager<AppUser> userManager, ClaimsPrincipal User)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var userAddress = await userManager.Users.FirstOrDefaultAsync(U => U.Email == userEmail);

            return userAddress;
        }
    }
}
