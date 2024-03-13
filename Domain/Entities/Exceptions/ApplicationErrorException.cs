using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunction.Domain.Entities.Exceptions
{
    public class ApplicationErrorException : Exception
    {
        public ApplicationErrorException(Exception ex)
        {
            this.ApplicationException = ex;
            this.ApplicationInnerException = ex?.InnerException;
            this.RaisedTime = DateTime.UtcNow;
        }
        public Exception ApplicationException { get; private set; }

        public Exception? ApplicationInnerException { get; private set; }

        public DateTime RaisedTime { get; set; }

        public override string StackTrace => this.ApplicationException.StackTrace ?? string.Empty;

        public override string Message => this.ApplicationException.Message;

        public string RequestId { get; set; } = string.Empty;

    }
}
