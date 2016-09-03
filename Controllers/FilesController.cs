using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

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

            if (Request.HasFormContentType)
            {
                if (TokenManager.HasToken(token))
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

                            string fileName = await FileManager.WriteFile(fileExtension, file, _appSettings.Value.UploadFolder);
                            fileUrls.Add($"{_appSettings.Value.HostUrl}/{_appSettings.Value.UploadFolder}/{fileName}");
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
    }
}
