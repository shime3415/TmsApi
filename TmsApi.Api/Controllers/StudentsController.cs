using TmsApi.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using TmsApi.Application.Interfaces;

namespace TmsApi.Api.Controllers;
[ApiController]
[Route("api/students")]
public class StudentsController : ControllerBase
{
    private readonly StudentService _studentService;

    public StudentsController(StudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetStudents(int page = 1)
    {
        var students = await _studentService.GetStudentsAsync(page);
        return Ok(students);
    }
}