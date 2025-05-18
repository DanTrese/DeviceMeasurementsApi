using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace DeviceMeasurementsApi.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
        }

        [HttpPost("/login")]
        public async Task<IActionResult> Login([FromForm] string username, [FromForm] string password)
        {
            _logger.LogInformation("POST /login");
            if (username == "admin" && password == "1234")
            {
                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, username)
                    };

                var identity = new ClaimsIdentity(claims, "Cookies");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("Cookies", principal);

                _logger.LogInformation("Успішний вхід");
                return Ok("Успішний вхід");
            }

            _logger.LogWarning("Невірні дані");
            return Unauthorized("Невірні дані");
        }

        [HttpPost("/logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            _logger.LogInformation("Вихід виконано");
            return Ok("Вихід виконано");
        }
    }
}
