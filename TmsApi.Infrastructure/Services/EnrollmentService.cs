using TmsApi.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using TmsApi.Infrastructure.Persistence;
using TmsApi.Application.Dtos;
using TmsApi.Domain.Entities;

namespace TmsApi.Infrastructure.Services;

public class EnrollmentService(TmsDbContext context, ILogger<EnrollmentService> logger) : IEnrollmentService
{
    public Task<EnrollmentResponseDto?> GetByIdAsync(int courseId, int id, CancellationToken ct) =>
        context.Enrollments
            .AsNoTracking()
            .Where(e => e.Id == id && e.CourseId == courseId)
            .Select(e => new EnrollmentResponseDto(e.Id, e.CourseId, e.StudentId, e.EnrolledAt))
            .FirstOrDefaultAsync(ct);

    public Task<List<EnrollmentResponseDto>> GetByCourseAsync(int courseId, CancellationToken ct) =>
        context.Enrollments
            .AsNoTracking()
            .Where(e => e.CourseId == courseId)
            .Select(e => new EnrollmentResponseDto(e.Id, e.CourseId, e.StudentId, e.EnrolledAt))
            .ToListAsync(ct);

    public async Task<bool> ExistsAsync(int studentId, string courseCode, CancellationToken ct) =>
        await context.Enrollments
            .AsNoTracking()
            .AnyAsync(e => e.StudentId == studentId && e.Course.Code == courseCode, ct);

    public async Task AddAsync(Enrollment enrollment, CancellationToken ct)
    {
        context.Enrollments.Add(enrollment);
        await context.SaveChangesAsync(ct);
        logger.LogInformation("Enrolled student {StudentId} in course {CourseId}", enrollment.StudentId, enrollment.CourseId);
    }

    public Task<List<Enrollment>> GetByStudentIdAsync(int studentId, CancellationToken ct) =>
        context.Enrollments
            .AsNoTracking()
            .Include(e => e.Course)
            .Where(e => e.StudentId == studentId)
            .ToListAsync(ct);

    public async Task<EnrollmentResponseDto> CreateAsync(int courseId, EnrollStudentRequest request, CancellationToken ct)
    {
        var enrollment = new Enrollment
        {
            CourseId = courseId,
            StudentId = request.StudentId,
            EnrolledAt = DateTime.UtcNow
        };

        context.Enrollments.Add(enrollment);
        await context.SaveChangesAsync(ct);

        logger.LogInformation("Enrolled student {StudentId} in course {CourseId}", request.StudentId, courseId);

        return (await GetByIdAsync(courseId, enrollment.Id, ct))!;
    }
}