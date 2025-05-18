using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DeviceMeasurementsApi.Controllers
{
    public class AuthController : ControllerBase
    {
        [HttpPost("/login")]
        public async Task<IActionResult> Login([FromForm] string username, [FromForm] string password)
        {
            // Простой пример (можно заменить на проверку в БД)
            if (username == "admin" && password == "1234")
            {
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username)
            };

                var identity = new ClaimsIdentity(claims, "Cookies");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("Cookies", principal);

                return Ok("Успішний вхід");
            }

            return Unauthorized("Невірні дані");
        }

        [HttpPost("/logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            return Ok("Вихід виконано");
        }
    }
}
