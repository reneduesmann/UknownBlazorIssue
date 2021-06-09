using Microsoft.Net.Http.Headers;
using System;
using System.IO;

namespace API
{
    public static class MultipartRequestHelper
    {
        public static bool IsMultipartContentType(string contentType)
        {
            return !string.IsNullOrWhiteSpace(contentType)
                   && contentType.Contains("multipart/", StringComparison.OrdinalIgnoreCase);
        }

        public static string GetBoundary(MediaTypeHeaderValue contentType, int lengthLimit)
        {
            string boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary).Value;

            if (string.IsNullOrWhiteSpace(boundary))
            {
                throw new InvalidDataException("Missing content-type boundary.");
            }

            return boundary.Length > lengthLimit
                ? throw new InvalidDataException($"Multipart boundary length limit {lengthLimit} exceeded.")
                : boundary;
        }

        public static bool HasFileContentDisposition(ContentDispositionHeaderValue contentDisposition)
        {
            // Content-Disposition: form-data; name="myfile1"; filename="Misc 002.jpg"
            return contentDisposition != null
                && contentDisposition.DispositionType.Equals("form-data")
                && (!string.IsNullOrEmpty(contentDisposition.FileName.Value)
                    || !string.IsNullOrEmpty(contentDisposition.FileNameStar.Value));
        }

        public static bool HasFormDataContentDisposition(ContentDispositionHeaderValue contentDisposition)
        {
            // Content-Disposition: form-data; name="key";
            return contentDisposition != null
                && contentDisposition.DispositionType.Equals("form-data")
                && string.IsNullOrEmpty(contentDisposition.FileName.Value)
                && string.IsNullOrEmpty(contentDisposition.FileNameStar.Value);
        }
    }
}
