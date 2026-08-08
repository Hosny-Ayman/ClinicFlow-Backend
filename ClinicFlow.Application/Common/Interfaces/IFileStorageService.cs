using Microsoft.AspNetCore.Http;

namespace ClinicFlow.Application.Common.Interfaces
{
    public interface IFileStorageService
    {

        Task<string> UploadImageAsync(IFormFile file);

        Task DeleteImageAsync(string publicId);

        string GetFileUrl(string publicId);

        string GetThumbnailUrl(string publicId, int width, int height);

    }
}
