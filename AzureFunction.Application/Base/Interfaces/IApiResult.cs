using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunction.Application.Base.Interfaces
{
    public interface IApiResult
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public int MessageNumber { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }

    public interface IResult<T> : IApiResult
    {
        T Data { get; set; }
    }
}
