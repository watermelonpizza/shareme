﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ShareMe
{
    public static class FileManager
    {
        public async static Task<string> WriteFile(string fileExtension, IFormFile file, string physicalUploadPath)
        {
            EnsureDirectory(physicalUploadPath);

            // Generate a file name, keep going if you get a collision with an existing file
            string fileName = string.Empty;
            do
            {
                fileName = $"{RandomGenerator.GetRandomString(7)}.{fileExtension}";
            } while (File.Exists(Path.Combine(physicalUploadPath, $"{fileName}.{fileExtension}")));

            try
            {
                string systemFilePath = Path.Combine(physicalUploadPath, fileName);

                using (var stream = File.Create(systemFilePath))
                {
                    await file.CopyToAsync(stream);
                }

                return fileName;
            }
            catch (Exception)
            {

                throw;
            }
        }
        
        public static void EnsureDirectory(string uploadFolder)
        {
            Directory.CreateDirectory(uploadFolder);
        }
    }
}
