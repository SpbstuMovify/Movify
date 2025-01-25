using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[Route("api/v1")]
[ApiController]
public class AuthController() : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login()
    {
        return Ok("sdfsdf");
    }

    [HttpPost("register")]
    public IActionResult Register()
    {
        return Ok();
    }
}
