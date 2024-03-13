namespace AzureFunction.Application.Base.Interfaces
{
    public interface ICallContext : IBaseData
    {
        string CorrelationId
        {
            get;
            set;
        }

        string FunctionName
        {
            get;
            set;
        }

        int? UserId
        {
            get;
            set;
        }

        string AuthenticationType
        {
            get;
            set;
        }

        IDictionary<string, string> AdditionalProperties
        {
            get;
        }

        string Response
        {
            get;
            set;
        }
    }
}
