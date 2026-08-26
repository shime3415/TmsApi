using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Asp.Versioning;

namespace TmsApi.Api.Controllers;

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/transcripts")]
public class TranscriptsController : ControllerBase
{
    [HttpPost]
    [EnableRateLimiting("transcripts")]
    public IActionResult RequestTranscript([FromBody] object? _)
    {
        // Stub: Exercise 5 replaces this with enqueue + 202 Accepted + Location header.
        return Ok();
    }
}