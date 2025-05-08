using Microsoft.EntityFrameworkCore;
using DeviceMeasurementsApi.Models;

namespace DeviceMeasurementsApi.Data
{
    public class MeasurementDbContext : DbContext
    {
        public MeasurementDbContext(DbContextOptions<MeasurementDbContext> options)
            : base(options)
        {
        }

        public DbSet<Measurement> Measurements { get; set; }


    }
}
