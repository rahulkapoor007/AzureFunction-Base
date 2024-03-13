using AzureFunction.Application.Base.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunction.Application.Base
{
    public class CallContext : ICallContext
    {
        public string CorrelationId { get; set; } = string.Empty;
        public int? UserId { get; set; } = 0;
        public string AuthenticationType { get; set; } = string.Empty;
        public string FunctionName { get; set; } = string.Empty;
        public IDictionary<string, string> AdditionalProperties { get; } = new Dictionary<string, string>();
        public string Response { get; set; } = string.Empty;
        public int LanguageId { get; set; }
        public int StatusCode { get; set; }
        
    }
}
