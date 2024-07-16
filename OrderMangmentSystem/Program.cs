
using OMS.Data.Middleware;
using OrderMangmentSystem.Helper;

namespace OrderMangmentSystem
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            ServiceMainHandling.ApplyServiceMainHandling(builder);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "OMS V1");
                });
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHsts(); 
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors("AllowOrigin");
            app.UseMiddleware<ServiceApiMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();



            // Seed the database
            await ApplySeedIngData.ApplySeeingDataAsync(app);
            await ApplySeedIngData.EnsureAppRolePopulated(app);
            await ApplySeedIngData.AddFunctionsToDatabase(app);
            await ApplySeedIngData.SyncFunctionRolesWithDatabase(app);

            await app.RunAsync();
        }
    }
}
