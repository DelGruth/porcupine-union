using Microsoft.EntityFrameworkCore;
using UserAccessSystem.Domain;

namespace UserAccessSystem.Internal.Persistence.DbContext;

public class UserAccessDbContext(DbContextOptions<UserAccessDbContext> opt)
    : Microsoft.EntityFrameworkCore.DbContext(options: opt)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Group> Groups { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureBaseDomainEntity(modelBuilder);
        ConfigureUserEntity(modelBuilder);
        ConfigureGroupEntity(modelBuilder);
        ConfigurePermissionEntity(modelBuilder);
        ConfigureUserPermissionGroupEntity(modelBuilder);
        ConfigureGroupPermissionEntity(modelBuilder);
        ConfigureUserPermissionEntity(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private static void ConfigureUserPermissionEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserPermission>(entity =>
        {
            entity.HasKey(x => new
            {
                x.UserId,
                x.PermissionId,
                x.IsDeleted,
            });

            entity.Property(x => x.UserId).IsRequired();
            entity
                .HasOne(x => x.User)
                .WithMany(x => x.UserPermissions)
                .HasForeignKey(x => x.PermissionId);

            entity
                .HasOne(x => x.Permission)
                .WithMany(x => x.UserPermissions)
                .HasForeignKey(x => x.UserId);
        });
    }

    private static void ConfigureGroupPermissionEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GroupPermission>(entity =>
        {
            entity.HasKey(x => new
            {
                x.GroupId,
                x.PermissionId,
                x.IsDeleted,
            });

            entity.Property(x => x.GroupId).IsRequired();
            entity
                .HasOne(x => x.Group)
                .WithMany(x => x.GroupPermissions)
                .HasForeignKey(x => x.PermissionId);

            entity
                .HasOne(x => x.Permission)
                .WithMany(x => x.GroupPermissions)
                .HasForeignKey(x => x.GroupId);
        });
    }

    private static void ConfigureUserPermissionGroupEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserGroupMembership>(entity =>
        {
            entity.HasKey(x => new
            {
                x.UserId,
                x.GroupId,
                x.IsDeleted,
            });

            entity.Property(x => x.UserId).IsRequired();
            entity.HasOne(x => x.User).WithMany(x => x.Groups).HasForeignKey(x => x.UserId);

            entity.Property(x => x.GroupId).IsRequired();
            entity.HasOne(x => x.Group).WithMany(x => x.Users).HasForeignKey(x => x.GroupId);
        });
    }

    private static void ConfigurePermissionEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Permission>(entity =>
        {
            entity.Property(x => x.Name).IsRequired().HasMaxLength(20);
            entity.Property(x => x.Description).IsRequired().HasMaxLength(150);
        });
    }

    private static void ConfigureGroupEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Group>(entity =>
        {
            entity.Property(x => x.Name).IsRequired().HasMaxLength(20);
            entity.Property(x => x.Description).HasMaxLength(150);
        });
    }

    private static void ConfigureUserEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(x => x.Email).IsRequired().HasMaxLength(60);
            entity.Property(x => x.Username).IsRequired().HasMaxLength(20);
            entity.HasIndex(x => x.Username).IsUnique();
            entity.Property(x => x.LockStatus).IsRequired().HasDefaultValue(LockStatus.None);
        });
    }

    private static void ConfigureBaseDomainEntity(ModelBuilder modelBuilder)
    {
        foreach (
            var entityType in modelBuilder
                .Model.GetEntityTypes()
                .Where(e => e.ClrType.IsSubclassOf(typeof(BaseDomainObj)))
        )
        {
            modelBuilder.Entity(entityType.ClrType).HasKey(nameof(BaseDomainObj.Id));

            modelBuilder
                .Entity(entityType.ClrType)
                .Property(nameof(BaseDomainObj.Version))
                .HasDefaultValue(1);

            modelBuilder
                .Entity(entityType.ClrType)
                .Property(nameof(BaseDomainObj.EditedById))
                .HasDefaultValue(Guid.Empty);

            modelBuilder
                .Entity(entityType.ClrType)
                .Property(nameof(BaseDomainObj.EditedDateTime))
                .HasDefaultValue(DateTime.UtcNow);

            modelBuilder
                .Entity(entityType.ClrType)
                .Property(nameof(BaseDomainObj.CreatedAtDateTime))
                .HasDefaultValue(DateTime.UtcNow);

            modelBuilder
                .Entity(entityType.ClrType)
                .Property(nameof(BaseDomainObj.IsDeleted))
                .HasDefaultValue(0);
        }
    }
}
