using Microsoft.AspNetCore.Http;

namespace VoiceMatters.Application.Services
{
    public interface IImageService
    {
        public Task<string> UploadFileAsync(IFormFile formFile);
        public Task DeleteByUuidAsync(string uuid);
    }
}
