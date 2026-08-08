using Microsoft.AspNetCore.Authentication;
using Module4.Authentication;
using Microsoft.EntityFrameworkCore;
using TmsApi.Data;
using TmsApi.Filters;
using TmsApi.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Standard RFC 7807 error responses for unhandled exceptions & status codes
builder.Services.AddProblemDetails();

builder.Services.AddOptions<PaymentOptions>()
    .BindConfiguration("Payments")
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddAuthentication("Training")
    .AddScheme<AuthenticationSchemeOptions, TrainingAuthHandler>(
        "Training",
        options => { });

// Register TmsDbContext scoped for incoming HTTP requests
builder.Services.AddDbContext<TmsDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("TmsDatabase")
    );

    options.LogTo(Console.WriteLine, LogLevel.Information);
});

builder.Services.AddAuthorization();

builder.Services.AddControllers(options =>
{options.Filters.Add<AuditLogFilter>();
});

// Required for OpenAPI document generation (needed by Scalar)
builder.Services.AddOpenApi();

builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<StudentService>();

var app = builder.Build();

// Turns unhandled exceptions into clean ProblemDetails (500) instead of raw stack traces
app.UseExceptionHandler();

// Turns bare error status codes (e.g. 404 with no body) into ProblemDetails responses
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();            // exposes /openapi/v1.json
    app.MapScalarApiReference(); // exposes /scalar/v1 UI
}

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<TmsDbContext>();
    await DataSeeder.SeedAsync(context);
}



app.UseMiddleware<RequestLoggingMiddleware>();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();