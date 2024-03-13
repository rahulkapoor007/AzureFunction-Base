using AzureFunction.FunctionApp.Extensions;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .AddInfrastructure()
    .Build();

host.Run();
