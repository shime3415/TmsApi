using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using TmsApi.Application.Dtos;
using TmsApi.Application.Interfaces;
using TmsApi.Domain.Exceptions; // <-- confirm/adjust this namespace once you find it
using TmsApi.Infrastructure.Caching;

namespace TmsApi.Infrastructure.Services;

public class CachedCourseService(
    HybridCache cache,
    ICourseService service,
    ILogger<CachedCourseService> logger)
    : ICachedCourseService
{
    public async Task<CourseResponseDto> GetCourseAsync(string code, CancellationToken ct)
    {
        var key = CacheKeys.Course(code);
        var dbHit = false;

        var dto = await cache.GetOrCreateAsync(
            key,
            (service, code),
            async (state, token) =>
            {
                dbHit = true;
                logger.LogInformation("Cache MISS for {Key} — fetching from DB", key);

                var course = await state.service.GetByCodeAsync(state.code, token)
                    ?? throw new NotFoundException($"Course {state.code} not found.");

                return new CourseResponseDto(
                    course.Id, course.Code, course.Title,
                    course.MaxCapacity, course.Enrollments.Count);
            },
            tags: [CacheKeys.CoursesTag],
            cancellationToken: ct);

        if (!dbHit)
            logger.LogInformation("Cache HIT for {Key}", key);

        return dto;
    }

    public async Task<List<CourseResponseDto>> GetAllCoursesAsync(CancellationToken ct)
    {
        var key = CacheKeys.CoursesAll;
        var dbHit = false;

        var list = await cache.GetOrCreateAsync(
            key,
            service,
            async (state, token) =>
            {
                dbHit = true;
                logger.LogInformation("Cache MISS for {Key} — fetching from DB", key);

                var allCourses = new List<CourseResponseDto>();
                var page = 1;
                PagedResponse<CourseResponseDto> result;

                do
                {
                    result = await state.GetCoursesAsync(
                        new PagedRequest { Page = page, PageSize = 50 }, token);
                    allCourses.AddRange(result.Items);
                    page++;
                }
                while (result.HasNext);

                return allCourses;
            },
            tags: [CacheKeys.CoursesTag],
            cancellationToken: ct);

        if (!dbHit)
            logger.LogInformation("Cache HIT for {Key}", key);

        return list;
    }

    public async Task InvalidateCourseCacheAsync(CancellationToken ct)
    {
        logger.LogInformation("Invalidating cache tag {Tag}", CacheKeys.CoursesTag);
        await cache.RemoveByTagAsync(CacheKeys.CoursesTag, ct);
    }
}