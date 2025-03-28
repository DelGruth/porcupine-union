using Microsoft.EntityFrameworkCore;
using UserAccessSystem.Domain;
using UserAccessSystem.Domain.Common;
using UserAccessSystem.Domain.Group;
using UserAccessSystem.Domain.Permissions;
using UserAccessSystem.Domain.User;
using UserAccessSystem.Domain.UserDetail;

namespace UserAccessSystem.Internal.Persistence.DbContext;

public class UserAccessDbContext(DbContextOptions<UserAccessDbContext> opt)
    : Microsoft.EntityFrameworkCore.DbContext(options: opt)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<GroupPermission> GroupPermissions { get; set; }
    public DbSet<UserPermission> UserPermissions { get; set; }
    public DbSet<UserGroupMembership> UserGroupMemberships { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureBaseDomainEntity(modelBuilder);
        ConfigureUserEntity(modelBuilder);
        ConfigureGroupEntity(modelBuilder);
        ConfigurePermissionEntity(modelBuilder);
        ConfigureUserPermissionGroupEntity(modelBuilder);
        ConfigureGroupPermissionEntity(modelBuilder);
        ConfigureUserPermissionEntity(modelBuilder);

        SeedData(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private static void ConfigureUserPermissionEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserPermission>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity
                .HasIndex(x => new
                {
                    x.UserId,
                    x.PermissionId,
                    x.GroupId,
                    x.IsDeleted,
                })
                .IsUnique();

            entity.Property(x => x.UserId).IsRequired();
            entity.Property(x => x.PermissionId).IsRequired();
            entity.Property(x => x.GroupId).IsRequired();

            entity
                .HasOne(x => x.User)
                .WithMany(x => x.UserPermissions)
                .HasForeignKey(x => x.UserId);

            entity
                .HasOne(x => x.Permission)
                .WithMany(x => x.UserPermissions)
                .HasForeignKey(x => x.PermissionId);

            entity
                .HasOne(x => x.Group)
                .WithMany(x => x.MemberPermissions)
                .HasForeignKey(x => x.GroupId);
        });
    }

    private static void ConfigureGroupPermissionEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GroupPermission>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity
                .HasIndex(x => new
                {
                    x.GroupId,
                    x.PermissionId,
                    x.IsDeleted,
                })
                .IsUnique();

            entity.Property(x => x.GroupId).IsRequired();
            entity.Property(x => x.PermissionId).IsRequired();

            entity
                .HasOne(x => x.Group)
                .WithMany(x => x.GroupPermissions)
                .HasForeignKey(x => x.GroupId);

            entity
                .HasOne(x => x.Permission)
                .WithMany(x => x.GroupPermissions)
                .HasForeignKey(x => x.PermissionId);
        });
    }

    private static void ConfigureUserPermissionGroupEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserGroupMembership>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity
                .HasIndex(x => new
                {
                    x.UserId,
                    x.GroupId,
                    x.IsDeleted,
                })
                .IsUnique();

            entity.Property(x => x.UserId).IsRequired();
            entity.Property(x => x.GroupId).IsRequired();

            entity.HasOne(x => x.User).WithMany(x => x.Groups).HasForeignKey(x => x.UserId);

            entity.HasOne(x => x.Group).WithMany(x => x.Users).HasForeignKey(x => x.GroupId);
        });
    }

    private static void ConfigurePermissionEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Permission>(entity =>
        {
            entity.Property(x => x.Name).IsRequired().HasMaxLength(20);
            entity.Property(x => x.Description).IsRequired().HasMaxLength(150);
            entity.Property(x => x.ReadOnly).IsRequired();
            entity.Property(x => x.WriteOnly).IsRequired();
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
            entity.Property(x => x.Password).IsRequired().HasMaxLength(60);
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

    private void SeedData(ModelBuilder modelBuilder)
    {
        var suId = Guid.NewGuid();

        modelBuilder
            .Entity<User>()
            .HasData(
                new User()
                {
                    Id = suId,
                    Username = "SU",
                    Password = "ecdsa",
                    Email = "null@undefined.com",
                    LockStatus = LockStatus.None,
                }
            );

        var allPermId = Guid.NewGuid();
        var superUserPermId = Guid.NewGuid();
        var accountLookupLevel1Id = Guid.NewGuid();
        var level2GeneralPermId = Guid.NewGuid();

        modelBuilder
            .Entity<Permission>()
            .HasData(
                new Permission
                {
                    Id = allPermId,
                    Name = "All",
                    Description = "All permissions",
                    ReadOnly = false,
                    WriteOnly = false,
                },
                new Permission
                {
                    Id = accountLookupLevel1Id,
                    Name = "AccountLookup_Level1",
                    Description = "Level 1 permission",
                    ReadOnly = true,
                    WriteOnly = false,
                },
                new Permission
                {
                    Id = superUserPermId,
                    Name = "SU",
                    Description = "Not so super user",
                    ReadOnly = false,
                    WriteOnly = true,
                },
                new Permission
                {
                    Id = level2GeneralPermId,
                    Name = "Level 2",
                    Description = "Poor user permission pack",
                    ReadOnly = true,
                    WriteOnly = true,
                }
            );

        var group1Id = Guid.NewGuid();
        var group2Id = Guid.NewGuid();

        modelBuilder
            .Entity<Group>()
            .HasData(
                new List<Group>()
                {
                    new Group
                    {
                        Id = group1Id,
                        Name = "Admins",
                        Description = "High Tower",
                    },
                    new Group
                    {
                        Id = group2Id,
                        Name = "CC",
                        Description = "Call center",
                    },
                }
            );

        modelBuilder
            .Entity<GroupPermission>()
            .HasData(
                new List<GroupPermission>()
                {
                    new GroupPermission
                    {
                        Id = Guid.NewGuid(),
                        GroupId = group1Id,
                        PermissionId = allPermId,
                    },
                    new GroupPermission
                    {
                        Id = Guid.NewGuid(),
                        GroupId = group2Id,
                        PermissionId = accountLookupLevel1Id,
                    },
                    new GroupPermission
                    {
                        Id = Guid.NewGuid(),
                        GroupId = group2Id,
                        PermissionId = level2GeneralPermId,
                    },
                }
            );

        modelBuilder
            .Entity<UserGroupMembership>()
            .HasData(
                new List<UserGroupMembership>
                {
                    new UserGroupMembership
                    {
                        Id = Guid.NewGuid(),
                        UserId = suId,
                        GroupId = group1Id,
                    },
                }
            );
    }
}
