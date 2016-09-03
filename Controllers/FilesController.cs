using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ShareMe.Controllers
{
    public class FilesController : Controller
    {
        // POST api/files
        [Route("api/upload")]
        [HttpPost]
        public async Task<string> Post(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return ReturnMessage.ErrorMessage("token not supplied");
            }

            if (TokenManager.HasToken(token) && Request.HasFormContentType)
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

                        string hostUrl = Environment.GetEnvironmentVariable("SHAREME_HOST_URL");
                        string filePath = await FileManager.WriteFile(fileExtension, file);
                        return ReturnMessage.OkFileUploaded("file uploaded", hostUrl + filePath);
                    }
                }
                catch (Exception)
                {

                    throw;
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
