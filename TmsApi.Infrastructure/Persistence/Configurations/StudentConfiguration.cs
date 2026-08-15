using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TmsApi.Domain.Entities;

namespace TmsApi.Infrastructure.Persistence.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.Property(s => s.Name).HasMaxLength(100);

        // --- Shadow property: audit timestamp ---
        // Not a C# property on Student.cs — EF tracks it internally,
        // you set it manually in the service layer before SaveChanges.
        builder.Property<DateTime>("LastUpdated");

        // --- Concurrency token ---
        // Npgsql maps IsRowVersion() to Postgres's built-in xmin system column.
        builder.Property(s => s.Version)
            .IsRowVersion();
    } // closes Configure()
} // closes the class