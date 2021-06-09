using Refit;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace WorkingExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string apiAddress = "https://localhost:5001";

            string pathForFileThatHaveUnderTwoGigabytes = @"";
            string pathForFileThatHaveMoreAsTwoGigabytes = @"";

            IFileApi api = RestService.For<IFileApi>(apiAddress);

            //using FileStream fileStream = new(pathForFileThatHaveUnderTwoGigabytes, FileMode.Open, FileAccess.Read); //Works
            using FileStream fileStream = new(pathForFileThatHaveMoreAsTwoGigabytes, FileMode.Open, FileAccess.Read); //Works

            StreamPart streamPart = new(fileStream, "filename.mov");

            try
            {
                HttpResponseMessage response = await api.UploadFileAsync(streamPart);
            }
            catch (Exception ex)
            {

            }
        }
    }
}
