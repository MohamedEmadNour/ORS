using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OMS.Data.Entites.Accounting;
using OMS.Data.Entites.System;

namespace OMS.Data.DBCOntext.Identity
{
    public class AppIdentityDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityUserRole<string>>(builder =>
            {
                builder.HasKey(ur => new { ur.UserId, ur.RoleId });

                builder.HasOne<AppRole>()
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.NoAction);

                builder.HasOne<AppUser>()
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<tbFunctionRoles>(builder =>
            {
                builder.HasKey(fr => new { fr.Id, fr.RoleId });

                builder.HasOne(fr => fr.tbFunctions)
                    .WithMany(f => f.tbFunctionRoles)
                    .HasForeignKey(fr => fr.Id);

                builder.HasOne(fr => fr.AppRole)
                    .WithMany(r => r.tbFunctionRoles)
                    .HasForeignKey(fr => fr.RoleId);
            });

     
        }
    

        public virtual DbSet<AppUser> AppUsers { get; set; }
        public virtual DbSet<tbFunctions> tbFunctions { get; set; }
        public virtual DbSet<tbFunctionRoles> tbFunctionRoles { get; set; }
        public virtual DbSet<AppRole> AppRole { get; set; }
    }
}
