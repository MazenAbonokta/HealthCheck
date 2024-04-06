using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Management;
using System.Net;
using System.ServiceProcess;

namespace AtlasHealthCheck
{
    public class CustomeServiceHealthCheck : IHealthCheck
    {
        HealthStatus status;
        String description;
   
        public CustomeServiceHealthCheck(HealthStatus Stateus, string Description)
        {
            status = Stateus;
            description = Description;
            
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {

            return new HealthCheckResult(status, description);

        }

    }
    
}