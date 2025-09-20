
using Feedback_System.Data;
using Feedback_System.Services;
using Microsoft.EntityFrameworkCore;

namespace Feedback_System
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //var connectionString = builder.Configuration.GetConnectionString("DBConnection");

            //builder.Services.AddDbContext<ApplicationDBContext>(options => {
            //    options.UseSqlServer(connectionString);
            //});


            var connection = String.Empty;
            if (builder.Environment.IsDevelopment())
            {
                builder.Configuration.AddEnvironmentVariables().AddJsonFile("appsettings.Development.json");
                connection = builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING");
            }
            else
            {
                connection = Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING");
            }

            builder.Services.AddDbContext<ApplicationDBContext>(options =>
                options.UseSqlServer(connection));

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddSingleton<PasswordServices>();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("allowall",
                    policy =>
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });
            });
            builder.Services.AddDbContext<ApplicationDBContext>(options =>
                  options.UseSqlServer(
                  builder.Configuration.GetConnectionString("DefaultConnection"),
                  sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
                  maxRetryCount: 5,           // Number of retry attempts
                  maxRetryDelay: TimeSpan.FromSeconds(10),  // Max delay between retries
                  errorNumbersToAdd: null     // You can add specific SQL error codes if needed
             )
             )
             );

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Enable static file serving
            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseCors("allowall");
            app.MapControllers();

            app.Run();
        }
    }
}
