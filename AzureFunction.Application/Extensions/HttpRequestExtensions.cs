using AzureFunction.Application.Base.Interfaces;
using HttpMultipartParser;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.Functions.Worker.Http;
using System.Reflection;
using System.Text.Json;

namespace AzureFunction.Application.Extensions
{
    public static class HttpRequestExtensions
    {
        public static async Task<TRequest> DeserializeAsync<TRequest>(this HttpRequestData httpRequest)
        {
            var comparer = EqualityComparer<TRequest>.Default;
            var dto = await httpRequest.DeserializeFromBodyAsync<TRequest>();

            if (!comparer.Equals(dto, default))
            {
                return dto;
            }

            dto = await httpRequest.DeserializeFromQueryStringAsync<TRequest>();
            return !comparer.Equals(dto, default)
                ? dto
                : Activator.CreateInstance<TRequest>();
        }

        private static async Task<TRequest> DeserializeFromQueryStringAsync<TRequest>(this HttpRequestData httpRequest)
        {
            await Task.CompletedTask;

            if (httpRequest?.Query == null)
            {
                return default;
            }

            var uri = httpRequest.Url;
            var queryDictionary = QueryHelpers.ParseQuery(uri.Query).ToDictionary(q => q.Key, q => q.Value);
            var convertedDictionary = new Dictionary<string, object>();

            foreach (var kvp in queryDictionary)
            {
                if (kvp.Value.Count == 1)
                {
                    convertedDictionary[kvp.Key] = ParseValue(kvp.Value.First());
                }
                else
                {
                    convertedDictionary[kvp.Key] = kvp.Value.Select(ParseValue).ToArray();
                }
            }
            // Serialize the dictionary to JSON
            var jsonString = System.Text.Json.JsonSerializer.Serialize(convertedDictionary, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // Optional: ignore case when deserializing
            });

            // Deserialize JSON to the specified type
            return System.Text.Json.JsonSerializer.Deserialize<TRequest>(jsonString);

        }

        private static object ParseValue(string value)
        {
            // Try to parse as JSON object
            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<object>(value);
            }
            catch
            {
                // Parse as string, int, double, etc. as needed
                // You can add additional parsing logic here based on the type of the value
                return value;
            }
        }
        private static async Task<TRequest> DeserializeFromBodyAsync<TRequest>(this HttpRequestData httpRequest)
        {

            if (httpRequest?.Body == null)
            {
                return default;
            }

            bool implementsIForm = typeof(TRequest).GetInterfaces().Contains(typeof(IFormData));
            implementsIForm |= typeof(TRequest).GetInterfaces().Contains(typeof(IFormDataList));
            if (implementsIForm)
            {
                TRequest request = Activator.CreateInstance<TRequest>();
                var parsedFormBody = await MultipartFormDataParser.ParseAsync(httpRequest.Body);
                var file = parsedFormBody.Files[0];
                foreach (var formDataEntity in parsedFormBody.Parameters)
                {
                    // Assuming that the property names in TRequest match the form field names
                    PropertyInfo property = typeof(TRequest).GetProperty(formDataEntity.Name);

                    if (property.PropertyType.IsEnum)
                    {
                        object value = Enum.ToObject(property.PropertyType, Enum.Parse(property.PropertyType, formDataEntity.Data));
                        property.SetValue(request, value);
                    }
                    else if (property != null)
                    {
                        // Convert the form data value to the appropriate property type
                        object value = Convert.ChangeType(formDataEntity.Data, property.PropertyType);
                        property.SetValue(request, value);
                    }
                }
                if (request is IFormData d)
                {
                    d.File = parsedFormBody.Files[0];
                }
                if (request is IFormDataList list)
                {
                    list.Files = parsedFormBody.Files.ToList();
                }
                return request;
            }

            var requestBody = await new StreamReader(httpRequest.Body).ReadToEndAsync();
            return requestBody.FromJson<TRequest>();
        }

        public static string Authorization(this HttpRequestData request) =>
            (request.Headers.TryGetValues("Authorization", out var authHeaderValues) && authHeaderValues.Any())
                ? authHeaderValues.First().Split(' ')[1] : string.Empty;

        public static string UserAgent(this HttpRequestData request) =>
            //request.Headers["User-Agent"].FirstOrDefault() ??
            string.Empty;

        public static string RemoteIpAddress(this HttpRequestData request) =>
            //request.HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ??
            string.Empty;
    }
}
