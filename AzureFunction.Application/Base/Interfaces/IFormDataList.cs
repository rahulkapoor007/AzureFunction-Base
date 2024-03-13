using HttpMultipartParser;

namespace AzureFunction.Application.Base.Interfaces
{
    public interface IFormDataList
    {
        public List<FilePart> Files { get; set; }
    }
}
