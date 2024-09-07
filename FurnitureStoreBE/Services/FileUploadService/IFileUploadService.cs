using CloudinaryDotNet.Actions;

namespace FurnitureStoreBE.Services.FileUploadService
{
    public interface IFileUploadService
    {
        Task<List<ImageUploadResult>> UploadFilesAsync(List<IFormFile> files, string folder);
        Task<DeletionResult> DestroyFileAsync(string publicId);

    }
}
