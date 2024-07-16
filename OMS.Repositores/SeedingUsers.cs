using Microsoft.AspNetCore.Identity;
using OMS.Data.DBCOntext.Identity;
using OMS.Data.Entites.Accounting;
using OMS.Data.Entites.Const;
using OMS.Data.Entites.System;

namespace OMS.Repositores
{
    public static class SeedingUsers
    {

        public static async Task ApplyUsersSeeding(UserManager<AppUser> users , RoleManager<AppRole> roleMgr , AppIdentityDbContext context)
        {
            if (roleMgr.Roles.Count(x => x.Name == RolesConstants.SuperAdmin) == 0)
            {
                await roleMgr.CreateAsync(new AppRole
                {
                    Name = RolesConstants.SuperAdmin
                });
            }
            if (!users.Users.Any())
            {
                AppUser user = new AppUser()
                {
                    UserName = "M.Emad",
                    Email = "MohamedEmad@outlook.com",
                    PhoneNumber = "01013952802",

                };

                var result = await users.CreateAsync(user, "P@$sw0rd");
                if (result.Succeeded)
                {
                    await users.AddToRoleAsync(user, RolesConstants.SuperAdmin);
                    await context.SaveChangesAsync();
                }
                else
                {
                    Console.WriteLine(result.Errors);
                }

                
            }

        }

    }
}
