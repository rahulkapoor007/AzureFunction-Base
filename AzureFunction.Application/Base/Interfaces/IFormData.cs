using HttpMultipartParser;

namespace AzureFunction.Application.Base.Interfaces
{
    public interface IFormData
    {
        public FilePart File { get; set; }
    }
}
