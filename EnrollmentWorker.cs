using TmsApi.Services;

public class EnrollmentWorker
{
    private readonly IServiceScopeFactory _scopeFactory;

    public EnrollmentWorker(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public void ProcessBatch()
    {
        using var scope = _scopeFactory.CreateScope();

        var enrollmentService =
            scope.ServiceProvider.GetRequiredService<IEnrollmentService>();

        // TODO: update this to use the current IEnrollmentService methods
        // (GetByIdAsync(courseId, id, ct) or CreateAsync(courseId, request, ct))
    }
}