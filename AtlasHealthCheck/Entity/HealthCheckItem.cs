using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtlasHealthCheck.Entity
{
    public  class HealthCheckItem
    {
        public HealthCheckItem() { }
        public string ServerIp { get; set; }
        public string ServerName { get; set; }
        public string CheckName { get; set; }
        public string Status { get; set; }
        public string LastCheck { get; set; }
        public string DisplayName { get; set; }
    }
}
