using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.ServiceProcess;

using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using AtlasHealthCheck;

public class WindowsServiceHealthCheck : IHealthCheck
{
    private readonly HealthCheckService1 _service;

    public WindowsServiceHealthCheck(HealthCheckService1 service)
    {
        _service = service;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var ss = _service.GetServices();
        return Task.FromResult(new HealthCheckResult(
            HealthStatus.Healthy));
    }
}