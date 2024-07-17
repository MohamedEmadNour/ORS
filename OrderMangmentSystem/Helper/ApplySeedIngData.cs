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
                var functionService = scope.ServiceProvider.GetRequiredService<IFunctionService>();
                var existingFunctions = await functionService.GetAllFunctionsAsync();
                var scannedFunctions = FunctionScanner.GetControllerAndActionNames();

                var existingFunctionNames = existingFunctions.Select(f => f.FunctionName).ToHashSet();
                var scannedFunctionNames = scannedFunctions.Select(f => f.FunctionName).ToHashSet();

                var functionsToAdd = scannedFunctions.Where(f => !existingFunctionNames.Contains(f.FunctionName)).ToList();
                var functionsToRemove = existingFunctions.Where(f => !scannedFunctionNames.Contains(f.FunctionName)).ToList();

                if (functionsToRemove.Any())
                {
                    dbContext.tbFunctions.RemoveRange(functionsToRemove);
                    await dbContext.SaveChangesAsync();
                }

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

                var superAdminRole = await dbContext.Roles.FirstOrDefaultAsync(r => r.Name == "SuperAdmin");
                var adminRole = await dbContext.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
                var userRole = await dbContext.Roles.FirstOrDefaultAsync(r => r.Name == "User");

                if (superAdminRole == null || adminRole == null || userRole == null)
                {
                    throw new Exception("Roles not found in the database.");
                }

                var existingFunctions = await functionService.GetAllFunctionsAsync();
                var existingFunctionRoles = await dbContext.tbFunctionRoles.ToListAsync();

                var existingFunctionRoleIds = existingFunctionRoles.ToLookup(fr => fr.tbFunctionsId);

                foreach (var function in existingFunctions)
                {
                    if (!existingFunctionRoleIds[function.tbFunctionsId].Any(fr => fr.RoleId == superAdminRole.Id))
                    {
                        dbContext.tbFunctionRoles.Add(new tbFunctionRoles
                        {
                            tbFunctionsId = function.tbFunctionsId,
                            RoleId = superAdminRole.Id,
                            IsDelete = false
                        });
                    }

                    if (function.IsAdminFunction &&
                        !existingFunctionRoleIds[function.tbFunctionsId].Any(fr => fr.RoleId == adminRole.Id))
                    {
                        dbContext.tbFunctionRoles.Add(new tbFunctionRoles
                        {
                            tbFunctionsId = function.tbFunctionsId,
                            RoleId = adminRole.Id,
                            IsDelete = false
                        });
                    }

                    if (function.IsUserFunction &&
                        !existingFunctionRoleIds[function.tbFunctionsId].Any(fr => fr.RoleId == userRole.Id))
                    {
                        dbContext.tbFunctionRoles.Add(new tbFunctionRoles
                        {
                            tbFunctionsId = function.tbFunctionsId,
                            RoleId = userRole.Id,
                            IsDelete = false
                        });
                    }
                }

                // Remove roles that are no longer valid
                var rolesToRemove = existingFunctionRoles
                    .Where(fr => !existingFunctions.Any(f => f.tbFunctionsId == fr.tbFunctionsId))
                    .ToList();

                if (rolesToRemove.Any())
                {
                    dbContext.tbFunctionRoles.RemoveRange(rolesToRemove);
                }

                await dbContext.SaveChangesAsync();
            }
        }

    }
}
