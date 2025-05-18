using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using DeviceMeasurementsApi.Models;


namespace DeviceMeasurementsApi.Services
{
    public class MeasurementSimulator : BackgroundService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public MeasurementSimulator(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var random = new Random();


            while (!stoppingToken.IsCancellationRequested)
            {
                var measurement = new Measurement
                {
                    Timestamp = DateTime.UtcNow,
                    Value = Math.Round(random.NextDouble() * 100, 2)
                };

                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("x-api-key", "Simulqtor_Privit");
                try
                {
                    await client.PostAsJsonAsync("https://localhost:7031/api/measurements", measurement, stoppingToken);
                }
                catch
                {
                    Console.WriteLine($"Fehler!");
                }

                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
