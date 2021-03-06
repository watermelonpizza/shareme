﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;

namespace ShareMe.Controllers
{
    public class FilesController : Controller
    {
        private readonly IOptions<AppSettings> _appSettings;

        public FilesController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        [HttpPost("api/files")]
        public async Task<string> Post(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return ReturnMessage.ErrorMessage("token not supplied");
            }
            
            if (Request.HasFormContentType)
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

        [HttpDelete("api/files/{filename}")]
        public async Task<string> Delete(string fileName, string token)
        {      
            if (string.IsNullOrWhiteSpace(token))
            {
                return ReturnMessage.ErrorMessage("token not supplied");
            }
            
            if (_appSettings.Value.AdminKey.Equals(token) || TokenManager.HasToken(token))
            {
                bool? result = FileManager.DeleteFile(fileName, _appSettings.Value.PhysicalUploadPath);
                if (result.HasValue)
                {
                    if (result.Value)
                    {
                        await CloudFlareManager.PurgeCache(
                            _appSettings.Value.CloudFlareZone,
                            _appSettings.Value.CloudFlareEmail,
                            _appSettings.Value.CloudFlareKey,
                            $"{_appSettings.Value.HostUrl}{_appSettings.Value.FileRequestPath}/{fileName}");

                        return ReturnMessage.OkFileDeleted($"file '{fileName}' successfuly deleted");
                    }
                    else
                        return ReturnMessage.ErrorMessage($"file '{fileName}' doesn't exist");
                }
                else
                    return ReturnMessage.ErrorMessage("could not delete file");
            }
            else
            {
                return ReturnMessage.ErrorMessage("unauthorised: invalid token");
            }
        }
    }
}