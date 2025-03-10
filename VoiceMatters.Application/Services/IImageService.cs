using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceMatters.Application.Services
{
    public interface IImageService
    {
        public Task<string> UploadFileAsync(IFormFile formFile);
        public Task DeleteByUuidAsync(string uuid);
    }
}
