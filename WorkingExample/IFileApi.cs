using Refit;
using System.Net.Http;
using System.Threading.Tasks;

namespace WorkingExample
{
    public interface IFileApi
    {
        [Multipart]
        [Post("/api/v1/File")]
        Task<HttpResponseMessage> UploadFileAsync(StreamPart streamPart);
    }
}
