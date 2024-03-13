using System.Net.Http.Headers;
using System.Net;
using Microsoft.Azure.Functions.Worker.Http;
using AzureFunction.Application.Base.Interfaces;

namespace AzureFunction.Application.Common
{
    public abstract partial class FunctionBase
    {
        protected static HttpResponseData Ok(object response, HttpRequestData req)
        {
            if (response != null && response is IApiResult ApiResponse)
            {
                var res = req.CreateResponse(ApiResponse.StatusCode);
                res.WriteAsJsonAsync(response ?? "");
                return res;
            }
            var resp = req.CreateResponse(HttpStatusCode.OK);
            resp.WriteAsJsonAsync(response ?? "");
            return resp;
        }

        protected static HttpResponseData Download(StreamContent content, IDownloadRequest request, HttpRequestData req)
        {
            var message = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = content
            };
            message.Content.Headers.ContentType = new MediaTypeHeaderValue(request.ContentType);
            message.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = request.Filename
            };
            return HttpResponseData.CreateResponse(req);
        }

        protected static HttpResponseData BadRequest(object response, HttpRequestData req)
        {
            if (response != null && response is IApiResult ApiResponse)
            {
                var res = req.CreateResponse(ApiResponse.StatusCode);
                res.WriteAsJsonAsync(response ?? "Bad request");
                return res;
            }
            var resp = req.CreateResponse(HttpStatusCode.BadRequest);
            resp.WriteAsJsonAsync(response ?? "Bad request");
            return resp;
        }

        protected static HttpResponseData NotFound(object response, HttpRequestData req)
        {
            if (response != null && response is IApiResult ApiResponse)
            {
                var res = req.CreateResponse(ApiResponse.StatusCode);
                res.WriteAsJsonAsync(response ?? "Not found");
                return res;
            }
            var resp = req.CreateResponse(HttpStatusCode.BadRequest);
            resp.WriteAsJsonAsync(response ?? "Not found");
            return resp;
        }

        protected static HttpResponseData Unauthorized(object response, HttpRequestData req)
        {
            if (response != null && response is IApiResult ApiResponse)
            {
                var res = req.CreateResponse(ApiResponse.StatusCode);
                res.WriteAsJsonAsync(response ?? "");
                return res;
            }
            var resp = req.CreateResponse(HttpStatusCode.Unauthorized);
            resp.WriteAsJsonAsync(response ?? "");
            return resp;
        }

        protected static HttpResponseData Forbidden(object response, HttpRequestData req)
        {
            if (response != null && response is IApiResult ApiResponse)
            {
                var res = req.CreateResponse(ApiResponse.StatusCode);
                res.WriteAsJsonAsync(response ?? "");
                return res;
            }
            var resp = req.CreateResponse(HttpStatusCode.Forbidden);
            resp.WriteAsJsonAsync(response ?? "");
            return resp;
        }

    }
}
