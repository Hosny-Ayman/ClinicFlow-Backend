namespace ClinicFlow.Application.Common.Interfaces
{
    public interface IFileStorageService
    {

        Task<string> UploadImageAsync(Stream stream, string fileName);

        Task DeleteImageAsync(string publicId);

        string GetFileUrl(string publicId);

        string GetThumbnailUrl(string publicId, int width, int height);

    }
}
