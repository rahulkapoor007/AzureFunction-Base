using AzureFunction.Application.Base.Interfaces;
using AzureFunction.Application.Common;
using AzureFunction.Application.Features.Users.CreateUsers;
using AzureFunction.Application.Features.Users.GetAllUsers;
using AzureFunction.Application.Features.Users.GetUserById;
using AzureFunction.Domain.Entities.Constants;
using AzureFunction.Domain.Entities.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ServiceStack;

namespace AzureFunction.FunctionApp.Functions
{
    public class UserApi : FunctionBase
    {
        private readonly ILogger<UserApi> _logger;
        private readonly CreateUsersHandler _createUsersHandler;
        private readonly GetAllUsersHandler _getAllUsersHandler;
        private readonly GetUserByIdHandler _getUserByIdHandler;

        public UserApi(
            ConfigWrapper config,
            ICallContext callContext,
            ILogger<UserApi> logger,
            CreateUsersHandler createUsersHandler,
            GetAllUsersHandler getAllUsersHandler,
            GetUserByIdHandler getUserByIdHandler) : base(config, callContext)
        {
            _logger = logger;
            _createUsersHandler = createUsersHandler;
            _getAllUsersHandler = getAllUsersHandler;
            _getUserByIdHandler = getUserByIdHandler;
        }

        [Function(nameof(CreateUsersAsync))]
        public async Task<HttpResponseData> CreateUsersAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "users")] HttpRequestData req, CancellationToken token)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            return await Handle<CreateUsersRequest, IResult<SuccessResponse>>(req, source => {
                return _createUsersHandler.Handle(source, token);
            });
        }

        [Function(nameof(GetAllUsersAsync))]
        public async Task<HttpResponseData> GetAllUsersAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users")] HttpRequestData req, CancellationToken token)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            return await Handle<GetAllUsersRequest, IResult<List<User>>>(req, source => {
                return _getAllUsersHandler.Handle(source, token);
            });
        }

        [Function(nameof(GetUserByIdAsync))]
        public async Task<HttpResponseData> GetUserByIdAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user")] HttpRequestData req, CancellationToken token)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            return await Handle<GetUserByIdRequest, IResult<User?>>(req, source => {
                return _getUserByIdHandler.Handle(source, token);
            });
        }
    }
}
