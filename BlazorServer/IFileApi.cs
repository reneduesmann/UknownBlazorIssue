using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorServer
{
    public interface IFileApi
    {
        [Multipart]
        [Post("/api/v1/File")]
        Task<HttpResponseMessage> UploadFileAsync(StreamPart streamPart);
    }
}
