using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Requires JWT token
public class ProtectedController : ControllerBase
{
    [HttpGet("data")]
    public IActionResult GetProtectedData()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var username = User.FindFirst(ClaimTypes.Name)?.Value;

        return Ok(new 
        { 
            message = "This is protected data",
            userId,
            username
        });
    }
}