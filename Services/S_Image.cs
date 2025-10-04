using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using TsrmWebApi.IServices;

namespace TsrmWebApi.Services
{


        public class S_Image : I_Image
        {
           
            private readonly ILogger<S_Image> _logger;
        private readonly string _imageFolderPath;
        private readonly string _imageFolderPathMarket;

        // private readonly string _imageFolderPath = @"E:\TSRM_FILE\TSRM_FILE_DOC";
        public S_Image(ILogger<S_Image> logger)
            {
                _logger = logger;
                //_connectionResolver = connectionResolver;
            _imageFolderPath = @"E:\TSRM_FILE\TSRM_FILE_DOC";
            _imageFolderPathMarket = @"E:\TSRM_FILE\TSRM_FILE_Market";

            if (!Directory.Exists(_imageFolderPath))
                Directory.CreateDirectory(_imageFolderPath);
        }


        public async Task<string> SaveImageAsync(IFormFile imageFile, int id)
        {
            if (imageFile == null || imageFile.Length == 0)
                throw new ArgumentException("Invalid image file");
            var originalNameWithoutExt = Path.GetFileNameWithoutExtension(imageFile.FileName);

            // File name format: ID + extension
            var fileExtension = Path.GetExtension(imageFile.FileName);
            //var fileName = $"{id}{fileExtension}";
            //var fileName = $"{originalNameWithoutExt}_{id}{fileExtension}";
            var fileName = $"{originalNameWithoutExt}{fileExtension}";
            var filePath = Path.Combine(_imageFolderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return filePath; // You can return relative path if you prefer
        }

        public FileStreamResult GetImageStream(string fileName)
        {
            var filePath = Path.Combine(_imageFolderPath, fileName);

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Image not found", fileName);

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return new FileStreamResult(stream, GetContentType(filePath));
        }


        public async Task<string> SaveImageMarketAsync(IFormFile imageFile, int id)
        {
            if (imageFile == null || imageFile.Length == 0)
                throw new ArgumentException("Invalid image file");
            var originalNameWithoutExt = Path.GetFileNameWithoutExtension(imageFile.FileName);

            // File name format: ID + extension
            var fileExtension = Path.GetExtension(imageFile.FileName);
            //var fileName = $"{id}{fileExtension}";
            //var fileName = $"{originalNameWithoutExt}_{id}{fileExtension}";
            var fileName = $"{originalNameWithoutExt}{fileExtension}";
            var filePath = Path.Combine(_imageFolderPathMarket, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return filePath; // You can return relative path if you prefer
        }

        public FileStreamResult GetImageMarketStream(string fileName)
        {
            var filePath = Path.Combine(_imageFolderPathMarket, fileName);

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Image not found", fileName);

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return new FileStreamResult(stream, GetContentType(filePath));
        }

        private string GetContentType(string path) => Path.GetExtension(path).ToLower() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };





    }

}
