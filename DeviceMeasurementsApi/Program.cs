using Microsoft.EntityFrameworkCore;
using DeviceMeasurementsApi.Data;
using DeviceMeasurementsApi.Services;
using Serilog;

namespace DeviceMeasurementsApi
{

    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<MeasurementDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddHttpClient();
            builder.Services.AddHostedService<MeasurementSimulator>();
            builder.Services.AddSingleton<StreamControlService>();
            builder.Host.UseSerilog();

            builder.Services.AddAuthentication("Cookies")
            .AddCookie("Cookies", options =>
            {
              options.LoginPath = "/login";
              options.AccessDeniedPath = "/denied";
             });


            builder.Services.AddControllersWithViews();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseStaticFiles();


            app.MapControllers();

            app.Run();
        }
    }
}
