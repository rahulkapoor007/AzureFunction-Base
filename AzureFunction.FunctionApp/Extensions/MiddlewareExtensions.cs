using AzureFunction.Application.Middlewares;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunction.FunctionApp.Extensions
{
    public static class MiddlewareExtensions
    {
        internal static IFunctionsWorkerApplicationBuilder UseMiddleware(this IFunctionsWorkerApplicationBuilder worker)
        {
            worker.UseWhen<HttpTriggerFunctionMiddleware>((context) =>
            {
                // We want to use this middleware only for http trigger invocations.
                return context.FunctionDefinition.InputBindings.Values
                              .First(a => a.Type.EndsWith("Trigger")).Type == "httpTrigger";
            });

            worker.UseWhen<ExceptionHandlingMiddleware>((context) =>
            {
                // We want to use this middleware only for http trigger invocations.
                return context.FunctionDefinition.InputBindings.Values
                              .First(a => a.Type.EndsWith("Trigger")).Type == "httpTrigger";
            });

            worker.UseWhen<RequestResponseLoggingMiddleware>((context) =>
            {
                // We want to use this middleware only for http trigger invocations.
                return context.FunctionDefinition.InputBindings.Values
                              .ToList().First(a => a.Type.EndsWith("Trigger")).Type == "httpTrigger";
            });
            return worker;
        }
    }
}
