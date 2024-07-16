using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OMS.Data.DBCOntext;
using OMS.Data.DBCOntext.Data;
using OMS.Data.DBCOntext.Identity;
using OMS.Data.Entites.Accounting;
using OMS.Data.Entites.Const;
using OMS.Data.Entites.System;
using OMS.Repositores;
using OMS.Service.Functions;
using OMS.Service.Utilites;

namespace OrderMangmentSystem.Helper
{
    public class ApplySeedIngData
    {
        public static async Task ApplySeeingDataAsync(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                var UserManger = serviceProvider.GetRequiredService<UserManager<AppUser>>();
                var roleMgr = serviceProvider.GetRequiredService<RoleManager<AppRole>>();
                try
                {
                    var context = serviceProvider.GetRequiredService<OSMDBContext>();
                    var Identity = serviceProvider.GetRequiredService<AppIdentityDbContext>();

                    await context.Database.MigrateAsync();
                    await Identity.Database.MigrateAsync();



   
                    await SeedingUsers.ApplyUsersSeeding(UserManger , roleMgr , Identity);

                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<ApplySeedIngData>();
                    logger.LogError(ex.Message);
                }
            }

        }

        public static async Task EnsureAppRolePopulated(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var roleMgr = serviceScope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

                if (!await roleMgr.RoleExistsAsync(RolesConstants.Admin))
                {
                    await roleMgr.CreateAsync(new AppRole { Name = RolesConstants.Admin });
                }
                if (!await roleMgr.RoleExistsAsync(RolesConstants.User))
                {
                    await roleMgr.CreateAsync(new AppRole { Name = RolesConstants.User });
                }
            }
        }
        public static async Task AddFunctionsToDatabase(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();
                var user = dbContext!.AppUsers.FirstOrDefault(u => u.UserName == "M.Emad");

                var functionService = scope.ServiceProvider.GetRequiredService<IFunctionService>();
                var existingFunctions = await functionService.GetAllFunctionsAsync();
                var scannedFunctions = FunctionScanner.GetControllerAndActionNames(user.Id);

                var existingFunctionNames = existingFunctions.Select(f => f.FunctionName).ToHashSet();
                var scannedFunctionNames = scannedFunctions.Select(f => f.FunctionName).ToHashSet();

                // Functions to add
                var functionsToAdd = scannedFunctions.Where(f => !existingFunctionNames.Contains(f.FunctionName)).ToList();

                // Functions to remove
                var functionsToRemove = existingFunctions.Where(f => !scannedFunctionNames.Contains(f.FunctionName)).ToList();

                // Remove obsolete functions
                if (functionsToRemove.Any())
                {
                    dbContext.tbFunctions.UpdateRange(functionsToRemove);
                    await dbContext.SaveChangesAsync();
                }

                // Add new functions
                if (functionsToAdd.Any())
                {
                    await functionService.AddFunctionsAsync(functionsToAdd);
                }
            }
        }
        public static async Task SyncFunctionRolesWithDatabase(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();
                var functionService = scope.ServiceProvider.GetRequiredService<IFunctionService>();

                // Get SuperAdmin role
                var superAdminRole = await dbContext.AppRole.FirstOrDefaultAsync(r => r.Name == "SuperAdmin");
                var adminUser = await dbContext.AppUsers.FirstOrDefaultAsync(u => u.UserName == "M.Emad");

                if (adminUser == null)
                {
                    throw new Exception("Admin user not found in the database.");
                }

                if (superAdminRole == null)
                {
                    throw new Exception("SuperAdmin role not found in the database.");
                }

                var existingFunctions = await functionService.GetAllFunctionsAsync();
                var existingFunctionRoleIds = dbContext.tbFunctionRoles
                                                        .Where(fr => fr.RoleId == superAdminRole.Id)
                                                        .ToHashSet();

                var functionsToAddRoles = existingFunctions
                                           .Where(f => !existingFunctionRoleIds.Select(s => s.Id).Contains(f.Id))
                                           .Select(f => new tbFunctionRoles
                                           {
                                               Id = f.Id,
                                               RoleId = superAdminRole.Id,
                                               IsDelete = false
                                           })
                                           .ToList();
                var functionsToAddRolesRemove = existingFunctionRoleIds.Where(existingFunctionRole => !existingFunctions.Select(s => s.Id).Contains(existingFunctionRole.Id));
                if (functionsToAddRoles.Any())
                {
                    await functionService.AddFunctionRolesAsync(functionsToAddRoles);
                }
                if (functionsToAddRolesRemove.Any())
                {
   
                    dbContext.tbFunctionRoles.UpdateRange(functionsToAddRolesRemove);
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
