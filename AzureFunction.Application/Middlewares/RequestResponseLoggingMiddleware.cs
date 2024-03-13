using AzureFunction.Application.Base.Interfaces;
using AzureFunction.Domain.Entities.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AzureFunction.Domain.Entities;
using AzureFunction.Application.Extensions;

namespace AzureFunction.Application.Middlewares
{
    public class RequestResponseLoggingMiddleware : IFunctionsWorkerMiddleware
    {
        private readonly ICallContext _context;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(
            ICallContext callContext,
            ILogger<RequestResponseLoggingMiddleware> logger)
            =>
                (_context, _logger) = (
                                        callContext ?? throw new ArgumentNullException(nameof(callContext)),
                                        logger ?? throw new ArgumentNullException(nameof(logger))
                                        );

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {

            // Retrieves the HTTP request data from the function context.
            var httpRequest = await context.GetHttpRequestDataAsync();

            if (httpRequest != null)
            {

                var requestResponseLogModel = new RequestResponseLogModel()
                {
                    Path = httpRequest.Url.AbsolutePath,
                    Method = httpRequest.Method,
                    QueryString = httpRequest.Url.Query,
                    RequestId = _context.CorrelationId
                    //IPAddress = context.Connection.RemoteIpAddress.ToString()
                };

                if (httpRequest.Method == "POST")
                {
                    // Leave the body open so the next middleware can read it.
                    using (var reader = new StreamReader(httpRequest.Body, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 4096, leaveOpen: true))
                    {
                        var body = await reader.ReadToEndAsync();
                        httpRequest.Body.Position = 0;
                        requestResponseLogModel.Payload = body;
                    }
                }

                requestResponseLogModel.RequestedOn = DateTime.UtcNow;
                var headersList = httpRequest.Headers.ToList();

                requestResponseLogModel.RequestHeaders = headersList?.ToJson() ?? string.Empty;

                try
                {
                    var authorization = httpRequest.Headers.GetValues(HeaderNames.Authorization).FirstOrDefault();
                    if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                    {
                        // we have a valid AuthenticationHeaderValue that has the following details:
                        var scheme = headerValue.Scheme;
                        var parameter = headerValue.Parameter;

                        // scheme will be "Bearer"
                        // parmameter will be the token itself.
                        requestResponseLogModel.JWTToken = parameter;
                        DecodeJWT decodeJWT = new DecodeJWT();
                        var claims = decodeJWT.GetClaims(requestResponseLogModel.JWTToken);
                        requestResponseLogModel.Email = claims.First(claim => claim.Type == "email").Value;
                        requestResponseLogModel.UserId = claims.First(claim => claim.Type == "user").Value;
                        _context.UserId = int.Parse(requestResponseLogModel.UserId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error Occured in Request Response Middleware", ex);
                }

                try
                {
                    // Calls the next function in the pipeline with the updated function context.
                    await next(context);

                }
                catch (Exception ex)
                {
                    _logger.LogError("Error Occured in Request Response Middleware", ex);

                    ApplicationErrorException applicationException;
                    if (ex is ApplicationErrorException) { applicationException = ex as ApplicationErrorException; }
                    else { applicationException = new ApplicationErrorException(ex); }

                    requestResponseLogModel.Response = "SOMETHING WENT WRONG...";
                    requestResponseLogModel.ResponseCode = StatusCodes.Status500InternalServerError.ToString();
                    requestResponseLogModel.IsSuccessStatusCode = false;
                    requestResponseLogModel.RespondedOn = DateTime.UtcNow;

                    _logger.LogInformation(requestResponseLogModel?.ToJson());

                    if (ex is ApplicationErrorException) throw;
                    else { throw new ApplicationErrorException(ex); }
                }

                requestResponseLogModel.RespondedOn = DateTime.UtcNow;
                var _contextService = (ICallContext)context.InstanceServices.GetService(typeof(ICallContext));
                requestResponseLogModel.Response = _contextService.Response;

                try
                {
                    // add the log object to the logger stream
                    _logger.LogInformation(requestResponseLogModel.ToJson());
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error Occured in Request Response Middleware", ex);

                }
            }
        }
    }
    public class DecodeJWT
    {
        public IEnumerable<Claim> GetClaims(string token)
        {
            try
            {
                var stream = token;
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(stream);
                var tokens = handler.ReadToken(stream) as JwtSecurityToken;
                return tokens.Claims;
            }
            catch (Exception ex)
            {
                if (ex is ApplicationErrorException) throw;
                else { throw new ApplicationErrorException(ex); }
            }
        }
    }
}
