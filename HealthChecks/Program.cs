using BetterHealthChecks.Core;
using BetterHealthChecks.Core.Models;
using BetterHealthChecks.Kafka;
using HealthChecks.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
    .AddBetterHealthChecks(x =>
    {
        //x.Add(new KafkaHealthCheck(new KafkaConfig("192.168.0.1"), "Kafka-PROD"));
        x.Add(new HttpHealthCheck("Localhost", new HttpConfig("http://localhost:2025", HttpMethod.Get, TimeSpan.FromSeconds(300))));
        x.Add(new HttpHealthCheck("Google", new HttpConfig("https://google.com", HttpMethod.Get, TimeSpan.FromSeconds(300)), false));
        return x;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.UseEndpoints(x => x.MapBetterHealthChecks("/health"));
app.Run();