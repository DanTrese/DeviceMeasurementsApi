using System;

namespace DeviceMeasurementsApi.Models
{
    public class Measurement
    {
        public int Id { get; set; }

        public DateTime Timestamp { get; set; }

        public double Value { get; set; }

    }
}
