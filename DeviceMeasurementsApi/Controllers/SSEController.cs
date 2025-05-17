using DeviceMeasurementsApi.Data;
using DeviceMeasurementsApi.Services;
using Microsoft.AspNetCore.Mvc;
using DeviceMeasurementsApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using System.Threading;
using System.Text.Json;

namespace DeviceMeasurementsApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class SSEController : ControllerBase
    {
        private readonly StreamControlService _streamControl;
        private readonly MeasurementDbContext _context;

        public SSEController(StreamControlService streamControl, MeasurementDbContext dbContext)
        {
            _streamControl = streamControl;
            _context = dbContext;
        }

        [HttpGet("sse")]
        public async Task GetSse(CancellationToken cancellationToken)
        {
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
                    await Response.WriteAsync("Not Found");
                    _streamControl.RequestStop();
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
            _streamControl.RequestStop();
            return Ok();
        }
    }
}
