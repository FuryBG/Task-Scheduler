using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {

        }

        public DbContext DbContext
        {
            get
            {
                return this;
            }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return base.SaveChangesAsync(cancellationToken);
        }

        public new DbSet<T> Set<T>() where T : class
        {
            return base.Set<T>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>().HasData(
                new Employee { Id = 1, Name = "Joe Bloggs" },
                new Employee { Id = 2, Name = "Ben Arnold" }
            );

            modelBuilder.Entity<Employee>()
            .HasMany(e => e.Roles)
            .WithMany(r => r.Employees)
            .UsingEntity(j => j.HasData(
                new { EmployeesId = 1, RolesId = 1 }, // Joe Bloggs - Waiter
                new { EmployeesId = 1, RolesId = 2 }, // Joe Bloggs - Barman
                new { EmployeesId = 2, RolesId = 3 }  // Ben Arnold - Chef
            ));

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Waiter" },
                new Role { Id = 2, Name = "Barman" },
                new Role { Id = 3, Name = "Chef" }
            );
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Role> Roles { get; set; }

    }
}
