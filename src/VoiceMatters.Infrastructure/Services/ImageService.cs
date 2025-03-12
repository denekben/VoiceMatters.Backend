using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using Uploadcare;
using Uploadcare.Upload;
using VoiceMatters.Application.Services;
using VoiceMatters.Shared.Exceptions;

namespace VoiceMatters.Infrastructure.Services
{
    public class ImageService : IImageService
    {
        private readonly FileUploader _fileUploader;
        private HttpClient _httpClient;
        private static string _publicKey;
        private static string _privateKey;

        public ImageService(IConfiguration config, HttpClient httpClient)
        {
            _publicKey = config["UploadcareSettings:PublicKey"];
            _privateKey = config["UploadcareSettings:PrivateKey"];

            var client = new UploadcareClient(
                _publicKey,
                _privateKey
                );

            _httpClient = httpClient;
            _fileUploader = new FileUploader(client);
        }

        public async Task<string> UploadFileAsync(IFormFile formFile)
        {
            if (formFile.Length > 0)
            {
                // Create a unique temporary file path
                var tempFilePath = Path.GetTempFileName();

                // Create a new file stream to write the IFormFile content to the temporary file
                using (var stream = new FileStream(tempFilePath, FileMode.Create))
                {
                    formFile.CopyTo(stream);
                }
                var fileInfo = new FileInfo(tempFilePath);
                var uploadedFile = await _fileUploader.Upload(fileInfo);

                return /*"https://ucarecdn.com/" +*/ uploadedFile.Uuid /*+ "/"*/;
            }
            else
            {
                throw new BadRequestException("IFormFile is empty");
            }
        }

        public async Task DeleteByUuidAsync(string uuid)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Uploadcare.Simple", $"{_publicKey}:{_privateKey}");

                // Установка заголовка Accept
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.uploadcare-v0.7+json"));

                // Выполнение запроса на удаление файла
                await _httpClient.DeleteAsync($"https://api.uploadcare.com/files/{uuid}/storage/");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
