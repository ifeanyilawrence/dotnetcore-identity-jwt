using Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid, ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin, ApplicationRoleClaim, ApplicationUserToken>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>().ToTable("ApplicationUser");
            modelBuilder.Entity<ApplicationRole>().ToTable("ApplicationRole");
            modelBuilder.Entity<ApplicationUserClaim>().ToTable("ApplicationUserClaim");
            modelBuilder.Entity<ApplicationUserRole>().ToTable("ApplicationUserRole");
            modelBuilder.Entity<ApplicationUserLogin>().ToTable("ApplicationUserLogin");
            modelBuilder.Entity<ApplicationRoleClaim>().ToTable("ApplicationRoleClaim");
            modelBuilder.Entity<ApplicationUserToken>().ToTable("ApplicationUserToken");
        }
    }
}
