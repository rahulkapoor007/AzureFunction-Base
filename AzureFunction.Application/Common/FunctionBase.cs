using AzureFunction.Application.Base.Interfaces;
using AzureFunction.Application.Extensions;
using AzureFunction.Domain.Entities.Constants;
using FluentValidation;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.IdentityModel.Tokens;
using ServiceStack;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace AzureFunction.Application.Common
{
    public abstract partial class FunctionBase
    {
        private readonly ConfigWrapper _config;
        private readonly ICallContext _callContext;

        public FunctionBase(ConfigWrapper config,
            ICallContext callContext)
        {
            _callContext = callContext;
            _config = config;
        }
        protected HttpRequestData? HttpRequest { get; set; }
        protected string EnvironmentName => _config.EnvironmentName;
        protected int UserId => 1;//_callContext.UserId ?? 1;//.GetValueOrDefault();

        protected async Task<TRequest> DeserializeAsync<TRequest>(HttpRequestData httpRequest)
        {
            HttpRequest = httpRequest;
            return await httpRequest.DeserializeAsync<TRequest>();
        }
        protected async Task<HttpResponseData> HandleAsync<TRequest, TResponse>(
            HttpRequestData httpRequest,
            Func<TRequest, Task<TResponse>> method,
            Action<TRequest> config = null)
        {
            var dto = await DeserializeAsync<TRequest>(httpRequest);
            config?.Invoke(dto);
            return await ProcessAsync(dto, method);
        }
        public async Task<HttpResponseData> Handle<TRequest, TResponse>(
         HttpRequestData httpRequest,
         Func<TRequest, Task<TResponse>> method,
         Action<TRequest> config = null) where TRequest : IReturn<TResponse>
        {
            var dto = await Prepare<TRequest>(httpRequest);
            config?.Invoke(dto);
            return await ProcessAsync(dto, method);
        }

        public async Task<HttpResponseData> HandleWithAuthentication<TRequest, TResponse>(
         HttpRequestData httpRequest,
         Func<TRequest, Task<TResponse>> method,
         Action<TRequest> config = null) where TRequest : IReturn<TResponse>
        {
            var dto = await Prepare<TRequest>(httpRequest);
            bool IsAuthenticated = await Authorization(httpRequest);
            config?.Invoke(dto);
            return await ProcessAsync(dto, method);
        }
        public virtual async Task<bool> Authorization(HttpRequestData httpRequest)
        {
            string bearerToken = httpRequest.Authorization();
            // JWT token validation configuration
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "your_issuer",
                ValidateAudience = true,
                ValidAudience = "your_audience",
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key")),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // No tolerance for time skew
            };

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(bearerToken, tokenValidationParameters, out validatedToken);
                return true;
                // Now you have a valid JWT token and its claims in the 'principal' variable
            }
            catch (SecurityTokenException ex)
            {
                // Token validation failed, handle the error
                // ex.Message will provide more information about the validation failure
                return false;
            }
        }
        public virtual async Task<TRequest> Prepare<TRequest>(HttpRequestData httpRequest)
        {
            HttpRequest = httpRequest;
            return await httpRequest.DeserializeAsync<TRequest>();
        }
        protected async Task<HttpResponseData> ProcessAsync<TRequest, TResponse>(TRequest dto,
            Func<TRequest, Task<TResponse>> method)
        {
            try
            {
                var response = await method(dto);
                if (dto is IDownloadRequest downloadRequest && response is IDownloadResponse downloadResponse)
                {
                    var content = await downloadResponse.GetContentAsync();
                    if (content != null)
                    {
                        return Download(content, downloadRequest, HttpRequest);
                    }
                }
                if (response is not null && response is IApiResult resultResponse)
                {
                    resultResponse.Message = await GetSystemMessageAsync(resultResponse.MessageNumber);
                    _callContext.StatusCode = (int)resultResponse.StatusCode;
                    _callContext.Response = resultResponse?.ToJson() ?? string.Empty;
                }
                var result = Ok(response, HttpRequest);                
                return result;
            }
            catch (Exception ex) when (
                ex is UnauthorizedAccessException
                || ex is ArgumentException
                || ex is NotSupportedException
            )
            {
                return HandleExpectedException(ex);
            }
        }

        protected async Task<HttpResponseData> ProcessAsync<TResponse>(Func<Task<TResponse>> method)
        {
            try
            {
                var response = await method();
                if (response != null && response.GetType() == typeof(HttpResponseMessage))
                {
                    throw new Exception("Can not return HttpResponseMessage");
                }
                if (response == null)
                {
                    return Forbidden(response, HttpRequest);
                }
                if (response != null && (
                    response.GetType() == typeof(Task) ||
                    response.GetType().IsGenericType &&
                    response.GetType().GetGenericTypeDefinition() == typeof(Task<>)))
                {
                    throw new Exception("Can not return Task, seems the `await` keyword is missing");
                }
                if (response is not null)
                {
                    //_callContext.Response = _serializer.Serialize(response);
                }
                var result = Ok(response, HttpRequest);
                return result;
            }
            catch (Exception ex) when (
                ex is UnauthorizedAccessException
                || ex is ArgumentException
                || ex is NotSupportedException
            )
            {
                return HandleExpectedException(ex);
            }
        }

        protected HttpResponseData HandleExpectedException(Exception exception)
        {
            return exception switch
            {
                UnauthorizedAccessException unauthorized => Unauthorized(new { unauthorized.Message }, HttpRequest),
                ArgumentException argument => BadRequest(new { argument.Message }, HttpRequest),
                NotSupportedException notSupportedException => BadRequest(new { notSupportedException.Message }, HttpRequest),
                _ => throw new Exception("To keep stack trace, don't catch this exception. Original message: " + exception.Message, exception)
            };
        }

        protected async Task<IApiResult?> ValidateAsync<TRequest>(
            TRequest dto,
            AbstractValidator<TRequest> validator,
            bool throwError = false)
        {
            if (dto == null)
            {
                throw new ArgumentNullException();
            }

            var result = await validator.ValidateAsync(dto);
            var error = result.Errors.FirstOrDefault();
            if (error == null)
            {
                return null;
            }

            if (!int.TryParse(error.ErrorCode ?? "1", out var code))
            {
                code = 1;
            }
            var errorMessage = await GetSystemMessageAsync(code);

            var res = AzureFunction.Application.Base.ApiResult.Failure(0);
            res.Message = errorMessage;
            return res;
        }

        protected async Task<string> GetSystemMessageAsync(int code)
        {
            if (code == 0)
            {
                return null;
            }

            return (SystemMessageSettings.Settings.ContainsKey(code)) ? SystemMessageSettings.Settings[code] : SystemMessageSettings.Settings[SystemMessageSettingNumbers.ValidationErrors];
        }
        protected async Task<string[]> GetSystemMessageAsync(int[] code)
        {
            if (code == null)
            {
                return null;
            }
            List<string> errors = new List<string>();

            foreach (var error in code)
            {
                errors.Add(
                    SystemMessageSettings.Settings.ContainsKey(error) ?
                    SystemMessageSettings.Settings[error]
                    : "");
            }

            return errors.ToArray();
        }

    }
}
