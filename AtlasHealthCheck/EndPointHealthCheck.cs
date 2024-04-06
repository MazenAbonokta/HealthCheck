using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AtlasHealthCheck
{
    public class EndPointHealthCheck
    {
        string url;
        RequestType requestType;
        AuthenticationType authenticationType;
        HttpStatusCode statusCode;
        string responseMessage;
        public EndPointHealthCheck(string url, RequestType requestType, AuthenticationType authenticationType, HttpStatusCode statusCode, string responseMessage)
        {
            this.url = url;
            this.requestType = requestType;
            this.authenticationType = authenticationType;
            this.statusCode = statusCode;
            this.responseMessage = responseMessage;
        }
    }
}
