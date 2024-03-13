using AzureFunction.Domain.Entities.Constants;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Http;
using AzureFunction.Application.Base;
using AzureFunction.Domain.Entities.Exceptions;

namespace AzureFunction.Application.Middlewares
{
    public class ExceptionHandlingMiddleware : IFunctionsWorkerMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            ILogger<ExceptionHandlingMiddleware> logger) =>
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case ApplicationErrorException:
                        await HandleException(context, exception: (ApplicationErrorException)ex);
                        break;
                    default:
                        ApplicationErrorException applicationException;
                        if (ex is ApplicationErrorException) { applicationException = ex as ApplicationErrorException; }
                        else { applicationException = new ApplicationErrorException(ex); }
                        await HandleException(context, exception: applicationException);
                        break;
                }
            }
        }
        private async Task HandleException(FunctionContext context, ApplicationErrorException exception)
        {

            var response = ApiResult.InternalServerError(0);
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.Succeeded = false;
            var httpRequest = await context.GetHttpRequestDataAsync();
            var resp = httpRequest.CreateResponse(HttpStatusCode.InternalServerError);
            resp.StatusCode = HttpStatusCode.InternalServerError;

            // Log the exception into database....
            string identifier = context.InvocationId;
            _logger.LogError(exception, "Error accrued", DateTime.UtcNow);
            _logger.LogError(exception.ApplicationInnerException, "Error accrued", DateTime.UtcNow);

            if (!string.IsNullOrEmpty(identifier))
            {
                // ABLE TO LOG EXCEPTION EITHER IN DATABASE OR IN FILE AND MAKING RESPONSE
                response.Message = string.Format(SystemMessageSettings.Settings[SystemMessageSettingNumbers.ExceptionMessageWithIdentifier], identifier);
                response.MessageNumber = SystemMessageSettingNumbers.ExceptionMessageWithIdentifier;
                await resp.WriteAsJsonAsync(response, resp.StatusCode);
            }
            else
            {
                // NOT ABLE TO LOG EXCEPTION NEITHER IN DATABASE NOR IN FILE AND MAKING RESPONSE
                response.Message = string.Format(SystemMessageSettings.Settings[SystemMessageSettingNumbers.ExceptionMessageWithTrace]
                    , exception.ApplicationException
                    , exception.InnerException
                    , exception.StackTrace);
                response.MessageNumber = SystemMessageSettingNumbers.ExceptionMessageWithIdentifier;
                await resp.WriteAsJsonAsync(response, resp.StatusCode);
            }
            resp.StatusCode = HttpStatusCode.InternalServerError;
            var invocationResult = context.GetInvocationResult();
            invocationResult.Value = resp;
        }
    }
}
