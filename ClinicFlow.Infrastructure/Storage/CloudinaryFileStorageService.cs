using ClinicFlow.Application.Common.Configurations;
using ClinicFlow.Application.Common.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ClinicFlow.Infrastructure.Storage
{
    public class CloudinaryFileStorageService : IFileStorageService
    {

        private readonly Cloudinary _cloudinary;
        private readonly ILogger<CloudinaryFileStorageService> _logger;

        public CloudinaryFileStorageService(IOptions<CloudinarySettings> options, ILogger<CloudinaryFileStorageService> logger)
        {
            var settings = options.Value;

            var account = new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret);

            _cloudinary = new Cloudinary (account);

            _logger = logger;
        }


        public async Task DeleteImageAsync(string publicId)
        {

            if (string.IsNullOrWhiteSpace(publicId))
                return;

            var deleteParams = new DeletionParams(publicId);

            var result = await _cloudinary.DestroyAsync(deleteParams);

            if(result.Result != "ok")
            {
                _logger.LogWarning("Failed to delete image {Image}", publicId);
            }

           
        }

        public string GetFileUrl(string publicId)
        {
            return _cloudinary.Api.UrlImgUp.BuildUrl(publicId);
        }

        public string GetThumbnailUrl(string publicId, int width, int height)
        {
            return _cloudinary.Api.UrlImgUp.Transform(new Transformation().Width(width).Height(height).Crop("fill")).BuildUrl(publicId);
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            await using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream)
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            return result.PublicId;
        }
    }
}
