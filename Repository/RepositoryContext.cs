using Entities.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Repository.Configuration;

// ReSharper disable UnusedMember.Global

namespace Repository;

// RepositoryContext now inherits from the IdentityDbContext class and not
// DbContext because we want to integrate our context with Identity
public class RepositoryContext : IdentityDbContext<User>
{
    public RepositoryContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // This is required for migration to work properly.
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new CompanyConfiguration());
        modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());

    }

    public DbSet<Company>? Companies { get; set; }

    public DbSet<Employee>? Employees { get; set; }
}