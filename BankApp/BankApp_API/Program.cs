using System.Text;
using System.Text.Json.Serialization;
using BankApp_API.services;
using BankApp_API.services.Models;
using BankApp_Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace BankApp_API;


public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        var configuration = builder.Configuration;
        builder.Services.AddDbContext<BankAppContext>(opt =>
            opt.UseSqlServer(configuration.GetConnectionString("BankAppEntityFrameworkWEBAPI"))
                .EnableSensitiveDataLogging()
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .EnableDetailedErrors()
            );
        
        
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();
        builder.Host.UseSerilog();
        

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        var appSettingsSection = builder.Configuration.GetSection("AppSettings");
        builder.Services.Configure<AppSettings>(appSettingsSection);
        var appSettings = appSettingsSection.Get<AppSettings>();
        var key = Encoding.ASCII.GetBytes(appSettings.Secret);
        builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve; 
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            // Or ReferenceHandler.IgnoreCycles for .NET 6+
        });
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<AdminService>();
        builder.Services.AddScoped<IAccountantService, AccountantService>();
      



        var app = builder.Build();
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BankAppContext>();

        var seeder = new AdminSeeder(context);
        seeder.Seed();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();
        Log.Information("🚀 API Successfully started!");

        try
        {
            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "❌ API failed to start properly");
        }
        finally
        {
            Log.CloseAndFlush();
        }

        app.Run();
    }
}