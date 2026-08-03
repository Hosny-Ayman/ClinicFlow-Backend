using ClinicFlow.Application.Common.Configurations;
using ClinicFlow.Application.Common.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ClinicFlow.Infrastructure.Storage
{
    public class CloudinaryFileStorageService : IFileStorageService
    {

        private readonly Cloudinary _cloudinary;

        public CloudinaryFileStorageService(IOptions<CloudinarySettings> options)
        {
            var settings = options.Value;

            var account = new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret);

            _cloudinary = new Cloudinary (account); 
        }


        public async Task<bool> DeleteImageAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);

            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result.Result == "ok";
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
