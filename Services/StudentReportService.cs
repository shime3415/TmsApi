using Microsoft.EntityFrameworkCore;
using TmsApi.Data;
using TmsApi.Entities;
using TmsApi.Services;
using Scalar.AspNetCore;
namespace TMS.Services
{
    public class StudentReportService
    {
        private readonly TmsDbContext _db;

        public StudentReportService(TmsDbContext db)
        {
            _db = db;
        }

        public async Task TestFixedQuery()
        {
            var report = await _db.Students
                .AsNoTracking()
                .Select(s => new
                {
                    s.Name,
                    EnrollmentCount = s.Enrollments.Count
                })
                .ToListAsync();

            foreach (var r in report)
            {
                Console.WriteLine($"{r.Name}: {r.EnrollmentCount} enrollments");
            }
        }
    }
}