using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunction.Domain.Entities
{
    public class RequestResponseLogModel
    {
        public RequestResponseLogModel()
        {
            PartitionKey = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
        }
        public long Id { get; set; }

        public string PartitionKey { get; }

        public string RequestId { get; set; }

        public string Path { get; set; }

        public string QueryString { get; set; }

        public string Method { get; set; }

        public string Payload { get; set; } = string.Empty;

        public string Response { get; set; }

        public string ResponseCode { get; set; }

        public DateTime RequestedOn { get; set; }

        public DateTime RespondedOn { get; set; }

        public bool IsSuccessStatusCode { get; set; }

        public string JWTToken { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string UserId { get; set; } = Guid.Empty.ToString();

        public string IPAddress { get; set; }

        public string RequestHeaders { get; set; }
    }
}
