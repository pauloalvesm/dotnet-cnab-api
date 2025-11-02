using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CNAB.Infra.Data.Context;

public class IdentityApplicationDbContext : IdentityDbContext
{
    public IdentityApplicationDbContext(DbContextOptions<IdentityApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
        modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins").HasKey(l => new { l.LoginProvider, l.ProviderKey });
        modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens").HasKey(t => new { t.UserId, t.LoginProvider, t.Name });
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles").HasKey(r => new { r.UserId, r.RoleId });
        modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");

        base.OnModelCreating(modelBuilder);
    }

}