using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;
using Urly.Application.Interfaces;
using Urly.Application.Mappings;
using Urly.Application.Services;
using Urly.Domain.Repositories;
using Urly.Infrastructure.Context;
using Urly.Infrastructure.Repositories;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

var myAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:5291",
                               "https://localhost:7032")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)
);

builder.Services.AddAutoMapper(typeof(ShortUrlMappingProfile).Assembly);

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUrlClickRepository, UrlClickRepository>();
builder.Services.AddScoped<IShortUrlRepository, ShortUrlRepository>();
builder.Services.AddScoped<IShortUrlService, ShortUrlService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy(policyName: "fixed-by-ip", httpContext =>
    {
        return RateLimitPartition.GetFixedWindowLimiter(

            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",

            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10, // Máximo de 10 requisições...
                Window = TimeSpan.FromSeconds(10), // ...a cada 10 segundos.
                QueueLimit = 0 // Sem fila
            });
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors(myAllowSpecificOrigins);
app.UseRateLimiter();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
