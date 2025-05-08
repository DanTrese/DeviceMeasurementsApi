using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DeviceMeasurementsApi.Models;
using System.Diagnostics.Metrics;
using DeviceMeasurementsApi.Data;

namespace DeviceMeasurementsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeasurementsController : ControllerBase
    {

        private readonly MeasurementDbContext _context;

        public MeasurementsController(MeasurementDbContext dbContext)
        {
            _context = dbContext;
        }

        [HttpPost]
        public IActionResult AddMeasurement([FromBody] Measurement measurement)
        {
            _context.Measurements.Add(measurement);

            _context.SaveChanges();

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
            var latest = _context.Measurements
            .OrderByDescending(m => m.Timestamp)
            .FirstOrDefault();

            if (latest == null)
                return NotFound();

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
                return BadRequest("Ungültige Messung!");
            }

            mesurementInDb.Timestamp = measurement.Timestamp;
            mesurementInDb.Value = measurement.Value;

            _context.SaveChanges();

            return Ok();
        }
    }
}
