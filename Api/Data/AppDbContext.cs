using Microsoft.EntityFrameworkCore;

using Api.Models;

namespace Api.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users
        => this.Set<User>();

    public DbSet<Project> Projects
        => this.Set<Project>();

    public DbSet<Employee> Employees
        => this.Set<Employee>();

    public DbSet<SalaryPayment> SalaryPayments
        => this.Set<SalaryPayment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(user => user.PublicId)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(user => user.Email)
            .IsUnique();

        modelBuilder.Entity<Project>()
            .HasIndex(project => project.PublicId)
            .IsUnique();

        modelBuilder.Entity<Project>()
            .HasOne(project => project.Manager)
            .WithMany()
            .HasForeignKey(project => project.ManagerId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Employee>()
            .HasIndex(employee => employee.PublicId)
            .IsUnique();

        modelBuilder.Entity<Employee>()
            .HasOne(employee => employee.Project)
            .WithMany(project => project.Employees)
            .HasForeignKey(employee => employee.ProjectId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<SalaryPayment>()
            .HasIndex(payment => payment.PublicId)
            .IsUnique();

        modelBuilder.Entity<SalaryPayment>()
            .HasOne(payment => payment.Employee)
            .WithMany()
            .HasForeignKey(payment => payment.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
