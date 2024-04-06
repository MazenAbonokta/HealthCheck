using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace AtlasHealthCheck
{
    public class EOCHealthCheck
    {
        IServiceCollection Service;
        Helper helper;

        public EOCHealthCheck(IServiceCollection _services)
        {
            Service = _services;
            helper = new Helper();
        }


        public void AddDiskStorageHealthCheck(int minimumFreeStorage)
        {
            //ObjectQuery query = new ObjectQuery($"SELECT * FROM Win32_LogicalDisk WHERE DeviceID='{driveLetter}'");
            ObjectQuery query = new ObjectQuery($"SELECT * FROM Win32_LogicalDisk");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            string Description = "";
            string ServerName = "";
            var ServiceStatus = HealthStatus.Healthy;
            foreach (ManagementObject disk in searcher.Get())
            {
                if (disk["Description"].ToString() == "Local Fixed Disk")
                {

                    ulong FullSize = Convert.ToUInt64(disk["Size"].ToString());
                    ulong FreeSpace = Convert.ToUInt64(disk["FreeSpace"]);
                    ulong percentageFreeSpace = (FreeSpace * 100 / FullSize);
                    if (Convert.ToUInt64(minimumFreeStorage) > percentageFreeSpace)
                    {


                    }
                    else
                    {
                        if (Convert.ToUInt64(minimumFreeStorage) > percentageFreeSpace)
                        {

                            ServiceStatus = HealthStatus.Healthy;


                        }
                    }
                    Description = $"Free Size {disk["DeviceID"]}  in Server {disk["SystemName"]} is {helper.FormatBytes(Convert.ToUInt64(disk["FreeSpace"]))} From {helper.FormatBytes(Convert.ToUInt64(disk["Size"]))}\n";
                    Description += $"Minimum Free Storage Percentage is {minimumFreeStorage}% \n";
                    Description += $"Actual Free Storage is {percentageFreeSpace}%";
                    ServerName = disk["SystemName"].ToString();
                    List<string> tags = new List<string>()
                                    { disk["SystemName"].ToString(),
                                      disk["DeviceID"].ToString(),


                                     };
                    Service.AddHealthChecks().AddCheck(ServerName + "_" + disk["DeviceID"], new CustomeServiceHealthCheck(ServiceStatus, Description), tags: tags);

                }


            }
        }

        public void AddRMemoryHealthCheck(int minimumFreeStorage)
        {
            ObjectQuery query = new ObjectQuery($"SELECT * FROM Win32_OperatingSystem");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            string Description = "";
            string ServerName = "";
            var ServiceStatus = HealthStatus.Healthy;
            foreach (ManagementObject obj in searcher.Get())
            {


                ulong FullSize = Convert.ToUInt64(obj["TotalVisibleMemorySize"].ToString());
                ulong FreeSpace = Convert.ToUInt64(obj["FreePhysicalMemory"]);
                ulong percentageFreeSpace = (FreeSpace * 100 / FullSize);
                if (Convert.ToUInt64(minimumFreeStorage) > percentageFreeSpace)
                {

                    ServiceStatus = HealthStatus.Unhealthy;

                }
                else
                {
                    ServiceStatus = HealthStatus.Healthy;

                }
                Description = $"Free Size Memory in Server {obj["CSName"]} with IP  is {helper.FormatKiloBytes(Convert.ToUInt64(obj["FreePhysicalMemory"]))} From {helper.FormatKiloBytes(Convert.ToUInt64(obj["TotalVisibleMemorySize"]))}\n";
                Description += $"Minimum Free Size Percentage is {minimumFreeStorage}% \n";
                Description += $"Actual Free Size is {percentageFreeSpace}%";
                ServerName = obj["CSName"].ToString();
                List<string> tags = new List<string>()
                                    { ServerName,

                                      "Memory"

                                     };
                Service.AddHealthChecks().AddCheck(ServerName + "_Momory", new CustomeServiceHealthCheck(ServiceStatus, Description), tags: tags);


            }



        }

        public async Task AddEndPointHealthCheckAsync(String url, RequestType requestType, AuthenticationType authenticationType, string username = null, string password = null, string BeareToken = null, string Key = null, String Value = null)
        {
            var client = new HttpClient();
           
            switch (authenticationType)
            {
                case AuthenticationType.ApiKey:
                    client.DefaultRequestHeaders.Add(Key, Key);
                    break;
                case AuthenticationType.BasicAuth:
                    string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                    break;
                case AuthenticationType.BeareToken:
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", BeareToken);
                    break;
                    

            }
            string messageResponse = "";
            HttpStatusCode statusCode;
            string Description = "";
            var EndPointStatus = HealthStatus.Unhealthy;

            HttpResponseMessage responseMessage=null;
            switch (requestType) {
                case RequestType.HttpPost:
                    string jsonInString = "[]";
                    responseMessage = await client.PostAsync(url, new StringContent(jsonInString, Encoding.UTF8, "application/json"));

                    break;
                    case RequestType.HttpPut:
                    break;
                    case RequestType.HttpDelete:
                    break;
                   
                case RequestType.HttpGet:
                    responseMessage = await client.GetAsync(url);
                    break;
                case RequestType.HttpPatch:
                    
                    break;
            }

            if(responseMessage !=null && responseMessage.IsSuccessStatusCode)
            {
                EndPointStatus= HealthStatus.Healthy;
            }
            messageResponse = await responseMessage.Content.ReadAsStringAsync();
            statusCode = responseMessage.StatusCode;
            List<string> tags = new List<string>()
                                    { requestType.ToString(),
                                    authenticationType.ToString(),
                                     statusCode.ToString(),
                                     GetRootDomain(url)

                                     };

            Description = "The End Point {" +url+ "} with request type " + requestType.ToString() + " the status code is { " +  responseMessage.StatusCode + "}";
            Service.AddHealthChecks().AddCheck(EndPointStatus + $"_{requestType.ToString()}", new CustomeServiceHealthCheck(EndPointStatus, Description), tags: tags);
        }
        private string GetRootDomain(string url)
        {
            try
            {
                Uri uri = new Uri(url);
                return uri.Host;
            }
            catch (UriFormatException ex)
            {
                Console.WriteLine("Invalid URL format: " + ex.Message);
                return "Invalid Url";
            }
        }
        public void AddApplicationPoolHealthCheck(string ApplicationPoolName)
        {
            ObjectQuery query = new ObjectQuery($"SELECT * FROM IIsApplicationPool WHERE Name = '{ApplicationPoolName}");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            string Description = "";
            string ServerName = "";
            var ServiceStatus = HealthStatus.Healthy;
            ManagementObjectCollection queryCollection = searcher.Get();
            if (queryCollection.Count > 0)
            {
                foreach (ManagementObject obj in queryCollection)
                {


                    string state = obj["AppPoolState"].ToString();
                    if (state != "Runinng")
                    {

                        ServiceStatus = HealthStatus.Unhealthy;

                    }
                    else
                    {
                        ServiceStatus = HealthStatus.Healthy;

                    }
                    Description = $"Application Pool {ApplicationPoolName} in Server {obj["CSName"]} is {state}\n";

                    ServerName = obj["CSName"].ToString();
                    List<string> tags = new List<string>()
                                    { ServerName,
                                    ApplicationPoolName,
                               
                                      "AppPoolState"

                                     };
                    Service.AddHealthChecks().AddCheck(ServerName + "_" + ApplicationPoolName, new CustomeServiceHealthCheck(ServiceStatus, Description), tags: tags);


                }
            }




        }
    }
  
    public enum RequestType
    {
        HttpGet,
        HttpPost,
        HttpDelete,
        HttpPut,
        HttpPatch,

    }
    public enum AuthenticationType
    {
        NoAuth,
        BasicAuth,
        BeareToken,
        ApiKey,



    }
  
}
