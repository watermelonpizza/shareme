using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Microsoft.AspNetCore.WebUtilities;
using System.IO;

namespace ShareMe.Controllers
{
    public class FilesController : Controller
    {
        private readonly IOptions<AppSettings> _appSettings;

        public FilesController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        // POST api/files
        [Route("api/upload")]
        [HttpPost]
        public async Task<string> Post(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return ReturnMessage.ErrorMessage("token not supplied");
            }

            if (IsMultipartContentType(Request.ContentType))
            {
                var boundary = GetBoundary(Request.ContentType);
                var reader = new MultipartReader(boundary, Request.Body);
                var section = await reader.ReadNextSectionAsync();
                string fileName = null;

                while (section != null)
                {
                    // process each image
                    const int chunkSize = 1024;
                    var buffer = new byte[chunkSize];
                    var bytesRead = 0;
                    fileName = GetFileName(section.ContentDisposition);

                    using (var stream = new FileStream(fileName, FileMode.Append))
                    {
                        do
                        {
                            bytesRead = await section.Body.ReadAsync(buffer, 0, buffer.Length);
                            stream.Write(buffer, 0, bytesRead);

                        } while (bytesRead > 0);
                    }

                    section = await reader.ReadNextSectionAsync();
                }

                return ReturnMessage.OkFileUploaded("file uploaded", new string[] { fileName });
            }
            else if (Request.HasFormContentType)
            {
                if (_appSettings.Value.AdminKey.Equals(token) || TokenManager.HasToken(token))
                {
                    try
                    {
                        List<string> fileUrls = new List<string>();
                        foreach (var file in Request.Form.Files)
                        {
                            string fileExtension = file.FileName.Split('.').Last();
                            
                            if (_appSettings.Value.ExtensionBlacklist.Contains(fileExtension))
                            {
                                return ReturnMessage.ErrorMessage($"upload rejected because of blacklisted file extension on {file.FileName}");
                            }

                            string fileName = await FileManager.WriteFile(fileExtension, file, _appSettings.Value.PhysicalUploadPath);
                            
                            fileUrls.Add($"{_appSettings.Value.HostUrl}{_appSettings.Value.FileRequestPath}/{fileName}");
                        }

                        return ReturnMessage.OkFileUploaded("file uploaded", fileUrls.ToArray());
                    }
                    catch (Exception e)
                    {
                        return ReturnMessage.ErrorMessage(e.Message);
                    }
                }
                else
                {
                    return ReturnMessage.ErrorMessage("unauthorised: invalid token");
                }
            }

            return ReturnMessage.ErrorMessage("no files or incorrect http post format encountered");
        }

        // DELETE api/values/12345.jpg
        [HttpDelete("{fileName}")]
        public void Delete(string fileName, string token)
        {
        }

        private static bool IsMultipartContentType(string contentType)
        {
            return
                !string.IsNullOrEmpty(contentType) &&
                contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static string GetBoundary(string contentType)
        {
            var elements = contentType.Split(' ');
            var element = elements.Where(entry => entry.StartsWith("boundary=")).First();
            var boundary = element.Substring("boundary=".Length);
            // Remove quotes
            if (boundary.Length >= 2 && boundary[0] == '"' &&
                boundary[boundary.Length - 1] == '"')
            {
                boundary = boundary.Substring(1, boundary.Length - 2);
            }
            return boundary;
        }

        private string GetFileName(string contentDisposition)
        {
            return contentDisposition
                .Split(';')
                .SingleOrDefault(part => part.Contains("filename"))
                .Split('=')
                .Last()
                .Trim('"');
        }
    }
}
