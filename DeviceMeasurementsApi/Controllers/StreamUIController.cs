using Microsoft.AspNetCore.Mvc;

namespace DeviceMeasurementApi.Controllers
{
    public class StreamUiController : Controller
    {
        [HttpGet("/connect")]
        public IActionResult Connect()
        {
            return View();
        }
    }
}