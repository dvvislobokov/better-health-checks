```csharp
builder.Services
    .AddBetterHealthChecks(x =>
    {
        x.Add(new KafkaHealthCheck(new KafkaConfig("localhost:9092"), "Kafka-Local"));
        x.Add(new HttpHealthCheck("Google", new HttpConfig("https://google.com", HttpMethod.Get, TimeSpan.FromSeconds(300)), false));
        x.Add(new PostgresHealthCheck("Server=localhost;Port=5434;Database=database;User Id=postgres;Password=pwd;Pooling=true;Minimum Pool Size=0;Maximum Pool Size=100;", TimeSpan.FromSeconds(10), null ));
        x.Add(new IgniteHealthCheck("Ignite-Local", "localhost:10800"));
        return x;
    });
    
    
 app.UseEndpoints(x => x.MapBetterHealthChecks("/health"));
```