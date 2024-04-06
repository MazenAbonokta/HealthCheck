using AtlasHealthCheck;
using HealthChecks.UI.Client;
using HealthChecks.UI.Data;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<HealthCheckService1>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks()
;
HealthCheckService1 _service =new HealthCheckService1();
var ss =await _service.GetServices();
Console.WriteLine(ss);

/*EOCRemoteServerHealthCheck remoteHealthCheck = new EOCRemoteServerHealthCheck(builder.Services, "10.100.10.191", "mazen", "770787mazen");
EOCHealthCheck healthCheck = new EOCHealthCheck(builder.Services);
//remoteHealthCheck.AddRemoteServiceHealthCheck("CM1PullService");
remoteHealthCheck.AddRemoteMemoryHealthCheck(36);
healthCheck.AddRMemoryHealthCheck(36);*/
/* .AddCheck("MMSServer", new RemoteServiceHealthCheck("10.100.10.191", "mazen", "770787mazen", "CM1PullService"));
*/
builder.Services.AddHealthChecksUI(s =>
{
    s.AddHealthCheckEndpoint("WebEOC-Health", "https://localhost:7200/WebEOC-Health");
    s.SetEvaluationTimeInSeconds(2);
}).AddInMemoryStorage();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecksUI();

app.MapHealthChecks("/WebEOC-Health", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
}); ;
app.Run();
