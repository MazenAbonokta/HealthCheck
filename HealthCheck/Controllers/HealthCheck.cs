using AtlasHealthCheck;
using k8s.Models;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Web.Administration;
using System.Security.Policy;
using System.Web;
namespace HealthCheck.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthCheck : ControllerBase
    {
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly HealthCheckService1 _service;
        public HealthCheck(IHostApplicationLifetime applicationLifetime, HealthCheckService1 healthCheckService)
        {
            _applicationLifetime = applicationLifetime;
            _service = healthCheckService;
        }

        [HttpGet ]
        [Route("RestartApplicationPool")]
        public IActionResult RestartApplicationPool(string servicename)
        {
           
            try
            {
              //  _service.Add(se);
                //      _applicationLifetime.StopApplication();
                return Ok("Application Pool restarted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest("An error occurred: " + ex.Message);
            }

        }
        static void Main()
        {
            // Specify the path to your web.config file
            string webConfigPath = @"C:\Path\To\Your\Published\Application\web.config";
            webConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "web.config");

            // Call the TouchWebConfig function
            TouchWebConfig(webConfigPath);
        }

        static void TouchWebConfig(string filePath)
        {
            try
            {
                // Open the web.config file in append mode and immediately close it
                using (FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write))
                {
                    fs.Close();
                }

                Console.WriteLine($"Touched {filePath}. This will trigger a restart of the application.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error touching web.config: {ex.Message}");
            }
        }

    }
}
