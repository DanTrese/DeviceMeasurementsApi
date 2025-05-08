using DeviceMeasurementsApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using System.Threading;

namespace DeviceMeasurementsApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class SSEController : ControllerBase
    {
        private readonly StreamControlService _streamControl;

        public SSEController(StreamControlService streamControl)
        {
            _streamControl = streamControl;
        }

        [HttpGet("sse")]
        public async Task GetSse(CancellationToken cancellationToken)
        {
            Response.Headers.Add("Content-Type", "text/event-stream");
            _streamControl.Reset();

            while (!cancellationToken.IsCancellationRequested && !_streamControl.ShouldStop)
            {
                var message = $"data: {DateTime.Now}\n\n";
                await Response.WriteAsync(message);
                await Response.Body.FlushAsync();
                await Task.Delay(1000, cancellationToken);
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
