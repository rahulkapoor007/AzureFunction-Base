using AzureFunction.Application.Base.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunction.Application.Base
{
    public class ApiResult : IApiResult
    {
        internal ApiResult(bool succeeded, int message, HttpStatusCode statusCode)
        {
            Succeeded = succeeded;
            MessageNumber = message;
            StatusCode = statusCode;
        }

        public bool Succeeded { get; set; }

        //public string[] Errors { get; set; }
        public int MessageNumber { get; set; }

        public string Message { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public static ApiResult Success(int message, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return new ApiResult(true, message, statusCode);
        }

        public static ApiResult Failure(int message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return new ApiResult(false, message, statusCode);
        }

        public static ApiResult InternalServerError(int message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            return new ApiResult(false, message, statusCode);
        }
    }

    public class Result<T> : ApiResult, IResult<T>
    {
        public Result(bool succeeded, int message, HttpStatusCode statusCode, T data) : base(succeeded, message, statusCode)
        {
            Data = data;
        }

        public T Data { get; set; }

        public static Result<T> Success(T data, int message, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return new Result<T>(true, message, statusCode, data);
        }

        public static Result<T> Failure(int message, T data, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return new Result<T>(false, message, statusCode, data);
        }

        public static Result<T> InternalServerError(int message, T data, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            return new Result<T>(false, message, statusCode, data);
        }

    }
}
