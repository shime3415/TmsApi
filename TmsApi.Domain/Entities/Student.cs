namespace TmsApi.Domain.Entities;

public class Student
{
    public int Id { get; set; }
    // surrogate primary key — internal, used by foreign keys

    public required string RegistrationNumber { get; set; }
    // natural key — human-readable (uniqueness configured in Session 2)

    public required string Name { get; set; }

    public decimal GPA { get; set; }

    public bool IsActive { get; set; } = true;

    // Concurrency token — maps to Postgres's xmin system column via Npgsql.
    // No default value needed; EF/Postgres manages this automatically.
    public uint Version { get; set; }

    // Navigation property for many-to-many relationship
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public bool IsDeleted { get; set; }
}