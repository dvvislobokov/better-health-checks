using BetterHealthChecks.Core;
using BetterHealthChecks.Core.Models;
using BetterHealthChecks.Ignite;
using BetterHealthChecks.Kafka;
using BetterHealthChecks.PostgreSQL;
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
        x.Add(new KafkaHealthCheck(new KafkaConfig("localhost:9092"), "Kafka-Local"));
        x.Add(new HttpHealthCheck("Google", new HttpConfig("https://google.com", HttpMethod.Get, TimeSpan.FromSeconds(300)), false));
        x.Add(new PostgresHealthCheck("Server=localhost;Port=5434;Database=oslms;User Id=postgres;Password=pwd;Pooling=true;Minimum Pool Size=0;Maximum Pool Size=100;", TimeSpan.FromSeconds(10), null ));
        x.Add(new IgniteHealthCheck("Ignite-Local", "localhost:10800"));
        return x;
    }, x=>x.Count(z=>z.Status==HealthStatus.Unhealthy)==1 ? 400 : 200);

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