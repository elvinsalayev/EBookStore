using EbookStore.Identity.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EbookStore.Identity.Data
{
    public class IdentityDbContext : IdentityDbContext<AppUser,
                                                       AppRole,
                                                       Guid,
                                                       AppUserClaim,
                                                       AppUserRole,
                                                       AppUserLogin,
                                                       AppRoleClaim,
                                                       AppUserToken>
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>(e => e.ToTable("Users", "Membership"));
            modelBuilder.Entity<AppRole>(e => e.ToTable("Roles", "Membership"));
            modelBuilder.Entity<AppUserClaim>(e => e.ToTable("UserClaims", "Membership"));
            modelBuilder.Entity<AppUserLogin>(e => e.ToTable("UserLogins", "Membership"));
            modelBuilder.Entity<AppUserRole>(e => e.ToTable("UserRoles", "Membership"));
            modelBuilder.Entity<AppRoleClaim>(e => e.ToTable("RoleClaims", "Membership"));
            modelBuilder.Entity<AppUserToken>(e => e.ToTable("UserTokens", "Membership"));
        }
    }
}
