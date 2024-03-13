using AzureFunction.Application.Base.Interfaces;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace AzureFunction.Application.Middlewares
{
    public class HttpTriggerFunctionMiddleware : IFunctionsWorkerMiddleware
    {
        private readonly ILogger<HttpTriggerFunctionMiddleware> _logger;
        private readonly ICallContext _context;

        public HttpTriggerFunctionMiddleware(ILogger<HttpTriggerFunctionMiddleware> logger, ICallContext callContext) =>
                (_logger, _context) = (logger ?? throw new ArgumentNullException(nameof(logger)),
                                        callContext ?? throw new ArgumentNullException(nameof(callContext)));

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            // Populate the context with information. This can be used by injecting the call context into any class
            this._context.CorrelationId = context.InvocationId.ToString();
            this._context.FunctionName = context.FunctionDefinition.Name;

            // Calls the next function in the pipeline with the updated function context.
            await next(context);

            var _contextService = (ICallContext)context.InstanceServices.GetService(typeof(ICallContext));
            if(_contextService != null && _contextService.StatusCode != 0)
            {
                var invocationResult = context.GetInvocationResult();
                var resp = context.GetHttpResponseData();
                resp.StatusCode = (HttpStatusCode)_contextService.StatusCode;
                invocationResult.Value = resp;
            }            
        }
    }
}
