using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace AtlasHealthCheck
{
    public class EOCRemoteServerHealthCheck
    {
        IServiceCollection Service;
        string RemoteServerIP;
        string Username;
        string Password;
        ManagementScope scope;
        Helper helper;
        public EOCRemoteServerHealthCheck(IServiceCollection service, string remoteServerIP, string username, string password)
        {
            Service = service;
            RemoteServerIP = remoteServerIP;
            Username = username;
            Password = password;

            ConnectionOptions connection = new ConnectionOptions
            {
                Username = Username,
                Password = Password,
                Impersonation = ImpersonationLevel.Impersonate,
                Authentication = AuthenticationLevel.PacketPrivacy
            };
            helper = new Helper();
            scope = new ManagementScope($"\\\\{RemoteServerIP}\\root\\CIMV2", connection);
            scope.Connect();
        }
        public void AddRemoteServiceHealthCheck(string ServiceName)
        {



            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_Service where Name = '" + ServiceName + "'");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);


            string Description = "";
            string ServerName = "";
            var ServiceStatus = HealthStatus.Healthy;

            foreach (ManagementObject myservice in searcher.Get())
            {
                ServiceStatus = myservice["State"] == "Running"
               ? HealthStatus.Healthy
               : HealthStatus.Unhealthy;
                Description = $"Service {ServiceName} is {myservice["State"]}  in Server {myservice["SystemName"]} with IP {RemoteServerIP}";
                ServerName = myservice["SystemName"].ToString();
                List<string> tags = new List<string>()
                { myservice["SystemName"].ToString(),
                  ServiceName,
                  RemoteServerIP,
                  myservice["DisplayName"].ToString(),
                };
                Service.AddHealthChecks().AddCheck(ServerName + "_" + ServiceName, new CustomeServiceHealthCheck(ServiceStatus, Description), tags: tags);
                return;


            }

            Service.AddHealthChecks().AddCheck(ServerName + "_" + ServiceName, new CustomeServiceHealthCheck(ServiceStatus, Description));

        }
        public void AddRemoteDiskStorageHealthCheck(int minimumFreeStorage)
        {
            //ObjectQuery query = new ObjectQuery($"SELECT * FROM Win32_LogicalDisk WHERE DeviceID='{driveLetter}'");
            ObjectQuery query = new ObjectQuery($"SELECT * FROM Win32_LogicalDisk");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
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
                    Description = $"Free Size {disk["DeviceID"]}  in Server {disk["SystemName"]} with IP {RemoteServerIP} is {helper.FormatBytes(Convert.ToUInt64(disk["FreeSpace"]))} From {helper.FormatBytes(Convert.ToUInt64(disk["Size"]))}\n";
                    Description += $"Minimum Free Storage Percentage is {minimumFreeStorage}% \n";
                    Description += $"Actual Free Storage is {percentageFreeSpace}%";
                    ServerName = disk["SystemName"].ToString();
                    List<string> tags = new List<string>()
                                    { disk["SystemName"].ToString(),
                                      disk["DeviceID"].ToString(),
                                      RemoteServerIP,

                                     };
                    Service.AddHealthChecks().AddCheck(ServerName + "_" + disk["DeviceID"], new CustomeServiceHealthCheck(ServiceStatus, Description), tags: tags);

                }


            }
        }
      
        public void AddRemoteMemoryHealthCheck(int minimumFreeStorage)
        {
            ObjectQuery query = new ObjectQuery($"SELECT * FROM Win32_OperatingSystem");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
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
                Description = $"Free Size Memory in Server {obj["CSName"]} with IP {RemoteServerIP} is {helper.FormatKiloBytes(Convert.ToUInt64(obj["FreePhysicalMemory"]))} From {helper.FormatKiloBytes(Convert.ToUInt64(obj["TotalVisibleMemorySize"]))}\n";
                Description += $"Minimum Free Size Percentage is {minimumFreeStorage}% \n";
                Description += $"Actual Free Size is {percentageFreeSpace}%";
                ServerName = obj["CSName"].ToString();
                List<string> tags = new List<string>()
                                    { ServerName,

                                      RemoteServerIP,
                                      "Memory"

                                     };
                Service.AddHealthChecks().AddCheck(ServerName + "_Momory", new CustomeServiceHealthCheck(ServiceStatus, Description), tags: tags);


            }



        }
        public void AddRemoteApplicationPoolHealthCheck(string ApplicationPoolName)
        {
            ObjectQuery query = new ObjectQuery($"SELECT * FROM IIsApplicationPool WHERE Name = '{ApplicationPoolName}");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            string Description = "";
            string ServerName = "";
            var ServiceStatus = HealthStatus.Healthy;
            ManagementObjectCollection queryCollection = searcher.Get();
            if (queryCollection.Count > 0)
            {
                foreach (ManagementObject obj in queryCollection)
                {


                    string state = obj["AppPoolState"].ToString();
                    if (state!="Runinng")
                    {

                        ServiceStatus = HealthStatus.Unhealthy;

                    }
                    else
                    {
                        ServiceStatus = HealthStatus.Healthy;

                    }
                    Description = $"Application Pool {ApplicationPoolName} in Server {obj["CSName"]} with IP {RemoteServerIP} is {state}\n";
                   
                    ServerName = obj["CSName"].ToString();
                    List<string> tags = new List<string>()
                                    { ServerName,
                                    ApplicationPoolName,
                                      RemoteServerIP,
                                      "AppPoolState"

                                     };
                    Service.AddHealthChecks().AddCheck(ServerName + "_" + ApplicationPoolName, new CustomeServiceHealthCheck(ServiceStatus, Description), tags: tags);


                }
            }
         



        }
    }
}
