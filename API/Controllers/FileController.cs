using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private static readonly FormOptions _defaultFormOptions = new();

        public FileController()
        {

        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadFileAsync()
        {
            if (!MultipartRequestHelper.IsMultipartContentType(this.Request.ContentType))
            {
                this.ModelState.AddModelError("File", "The request couldn't be processed (Error 1).");

                return this.BadRequest(this.ModelState);
            }

            string boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(this.Request.ContentType), _defaultFormOptions.MultipartBoundaryLengthLimit);

            MultipartReader reader = new(boundary, this.HttpContext.Request.Body);

            MultipartSection section = await reader.ReadNextSectionAsync();

            while (section != null)
            {
                bool hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(
                section.ContentDisposition, out ContentDispositionHeaderValue contentDisposition);

                if (hasContentDispositionHeader)
                {
                    if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                    {
                        try
                        {
                            string trustedFileName = WebUtility.HtmlEncode(contentDisposition.FileName.Value);

                            //only demo
                            using FileStream fs = new("test", FileMode.OpenOrCreate, FileAccess.ReadWrite);

                            int bytesRead = 0;
                            byte[] buffer = new byte[8192];

                            while ((bytesRead = await section.Body.ReadAsync(buffer.AsMemory(0, buffer.Length))) != 0)
                            {
                                //in my normal project, i write the stream to a mongo database, but this is working.
                                //but sometimes, when the file from the blazor app reach the api endpoint
                                //it jumps over this while loop. but i dont know why too.
                                await fs.WriteAsync(buffer.AsMemory(0, bytesRead));
                            }

                            fs.Close();
                            await section.Body.DisposeAsync();
                        }
                        catch (Exception ex)
                        {
                            this.ModelState.AddModelError("Processing", ex.ToString());
                        }
                    }
                    else
                    {
                        this.ModelState.AddModelError("File", "The request couldn't be processed (Error 2).");
                    }
                }

                section = await reader.ReadNextSectionAsync();
            }

            return this.Ok(this.ModelState);
        }
    }
}
