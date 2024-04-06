using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebEOC_API;

namespace AtlasHealthCheck
{
    public class HealthCheckService1
    {
        private readonly APISoapClient webEOC_API = new APISoapClient(APISoapClient.EndpointConfiguration.APISoap);
        private readonly WebEOCCredentials webEOCCredentials = new WebEOCCredentials();
        public async Task<string> GetServices()
        {
            webEOCCredentials.Username = "mazen";
            webEOCCredentials.Password = "123";
            webEOCCredentials.Position = "TestPosition";
            webEOCCredentials.Incident = "Day To Day";

            string response = await  webEOC_API.GetDataAsync(webEOCCredentials, "Health Check", "List - HealthCheck");
            return response;
            
        }

       
    }
}
