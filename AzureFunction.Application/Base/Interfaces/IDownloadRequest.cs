using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunction.Application.Base.Interfaces
{
    public interface IDownloadRequest
    {
        public string Filename { get; set; }
        public string ContentType { get; set; }
    }
    public interface IDownloadResponse
    {
        public Task<StreamContent> GetContentAsync();
    }
}
