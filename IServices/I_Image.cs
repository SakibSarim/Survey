using Microsoft.AspNetCore.Mvc;

namespace TsrmWebApi.IServices
{
    public interface I_Image
    {
        //public Task<string> UploadImageAsyncUnitWise(IFormFile file, decimal unitId,decimal productid);
    

        Task<string> SaveImageAsync(IFormFile imageFile, int id);

        FileStreamResult GetImageStream(string fileName);

        Task<string> SaveImageMarketAsync(IFormFile imageFile, int id);

        FileStreamResult GetImageMarketStream(string fileName);
    }
}
