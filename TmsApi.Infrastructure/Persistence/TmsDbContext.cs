using Microsoft.EntityFrameworkCore;
using TmsApi.Domain.Entities;

namespace TmsApi.Infrastructure.Persistence;

public class TmsDbContext(DbContextOptions<TmsDbContext> options)
    : DbContext(options)
{
    public DbSet<Student> Students => Set<Student>();

    public DbSet<Course> Courses => Set<Course>();

    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    // Extended Exercise
    public DbSet<Assessment> Assessments => Set<Assessment>();

    public DbSet<Certificate> Certificates => Set<Certificate>();

   // TmsDbContext.cs
    protected override void OnModelCreating(ModelBuilder modelBuilder)
     {
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(TmsDbContext).Assembly);
    // that's it — EF finds all your config classes automatically
      }
}