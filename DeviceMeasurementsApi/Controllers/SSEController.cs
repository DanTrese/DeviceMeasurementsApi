using DeviceMeasurementsApi.Data;
using DeviceMeasurementsApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace DeviceMeasurementsApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SSEController : ControllerBase
    {

        private readonly ILogger<SSEController> _logger;
        private readonly StreamControlService _streamControl;
        private readonly MeasurementDbContext _context;

        public SSEController(StreamControlService streamControl, MeasurementDbContext dbContext, ILogger<SSEController> logger)
        {
            _streamControl = streamControl;
            _context = dbContext;
            _logger = logger;
        }


        [HttpGet("sse")]
        public async Task GetSse(CancellationToken cancellationToken)
        {
            _logger.LogInformation("GET /api/sse/sse");
            Response.Headers.Add("Content-Type", "text/event-stream");
            Response.Headers.Add("Connection", "keep-alive");
            _streamControl.Reset();

            while (!cancellationToken.IsCancellationRequested && !_streamControl.ShouldStop)
            {
                var latest = _context.Measurements
                .OrderByDescending(m => m.Timestamp)
                .FirstOrDefault();
                if (latest == null)
                {
                    _logger.LogInformation("Вимірювань не знайдено");
                    await Response.WriteAsync("Not Found");
                    _logger.LogInformation("Стрим завершено");
                    _streamControl.RequestStop();
                    break;
                }
                var json = JsonSerializer.Serialize(latest);
                var message = $"data: {json}\n\n";

                await Response.WriteAsync(message);
                await Response.Body.FlushAsync();
                await Task.Delay(5000, cancellationToken);
            }
        }

        [HttpPost("stop")]
        public IActionResult StopStream()
        {
            _logger.LogInformation("Стрим завершено");
            _streamControl.RequestStop();
            return Ok();
        }
    }
}
