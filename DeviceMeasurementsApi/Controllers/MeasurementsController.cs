using Microsoft.AspNetCore.Mvc;
using DeviceMeasurementsApi.Models;
using DeviceMeasurementsApi.Data;
using DeviceMeasurementsApi.Services;
using Microsoft.AspNetCore.Authorization;

namespace DeviceMeasurementsApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MeasurementsController : ControllerBase
    {
        private readonly ILogger<MeasurementsController> _logger;
        private readonly StreamControlService _streamControl;
        private readonly MeasurementDbContext _context;

        public MeasurementsController(StreamControlService streamControl, MeasurementDbContext dbContext, ILogger<MeasurementsController> logger)
        {
            _streamControl = streamControl;
            _context = dbContext;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult AddMeasurement([FromBody] Measurement measurement)
        {
            _logger.LogInformation("POST /api/measurements");

            var key = Request.Headers["x-api-key"].FirstOrDefault();
            if (key != "Simulqtor_Privit")
                return Unauthorized("Невірний ключ");

            _context.Measurements.Add(measurement);
            _context.SaveChanges();

            _logger.LogInformation($"Вимірювання {measurement.Id} збережено у БД");
            return CreatedAtAction(nameof(AddMeasurement), new { measurement.Id }, measurement);
        }


        [HttpGet]
        public IActionResult GetMeasurement()
        {
            return Ok(_context.Measurements.ToList());
        }

        [HttpGet("{id}")]
        public IActionResult GetMeasurementById(int id)
        {
            var measurement = _context.Measurements.Find(id);
            if (measurement == null)
                return NotFound();
            return Ok(measurement);
        }

        [HttpGet("latest")]
        public IActionResult GetLatestMeasurement()
        {
            _logger.LogInformation("GET /api/measurements/latest");

            var latest = _context.Measurements
            .OrderByDescending(m => m.Timestamp)
            .FirstOrDefault();

            if (latest == null) {
                _logger.LogInformation("Вимірювань не знайдено");
                return NotFound();
            }
                

            _logger.LogInformation($"Останне вимірювання відправлено кліенту");
            return Ok(latest);
        }

        [HttpDelete]
        public IActionResult DeleteMeasurement(int id)
        {
            var measurement = _context.Measurements.Find(id);

            if (measurement == null)
            {
                return NotFound();
            }

            _context.Measurements.Remove(measurement);
            _context.SaveChanges();

            return NoContent();
        }
        [HttpPut]
        public IActionResult PutMeasurement(int id, [FromBody] Measurement measurement)
        {
            var mesurementInDb = _context.Measurements.Find(id);

            if (mesurementInDb == null)
            {
                return NotFound();
            }
            else if (measurement.Value < 0)
            {
                return BadRequest();
            }

            mesurementInDb.Timestamp = measurement.Timestamp;
            mesurementInDb.Value = measurement.Value;

            _context.SaveChanges();

            return Ok();
        }
    }
}
