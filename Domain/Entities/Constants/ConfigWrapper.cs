namespace AzureFunction.Domain.Entities.Constants
{
    public class ConfigWrapper
    {
        public string EnvironmentName
        {
            get { return Environment.GetEnvironmentVariable("EnvironmentName"); }
        }

    }
}
