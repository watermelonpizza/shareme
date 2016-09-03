using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

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
                        foreach (var file in Request.Form.Files)
                        {
                            string fileExtension;
                            string[] fileSplit = file.FileName.Split('.');

                            if (fileSplit.Length > 1)
                            {
                                fileExtension = fileSplit.Last();
                            }
                            else
                            {
                                fileExtension = string.Empty;
                            }
                            
                            string filePath = await FileManager.WriteFile(fileExtension, file);
                            return ReturnMessage.OkFileUploaded("file uploaded", _appSettings.Value.HostUrl + filePath);
                        }
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
