using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FurnitureStoreBE.Utils;
using Microsoft.Extensions.Options;

namespace FurnitureStoreBE.Services.FileUploadService
{ 
    public class FileUploadServiceImp : IFileUploadService
    {
        private readonly Cloudinary _cloudinary;
        public FileUploadServiceImp(IOptions<CloudinarySettings> config)
        {
            var account = new Account
            (
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(account);
        }
        public async Task<List<ImageUploadResult>> UploadFilesAsync(List<IFormFile> files, string folder)
        {
            var uploadResults = new List<ImageUploadResult>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    using var stream = file.OpenReadStream();
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        Transformation = new Transformation().Height(500).Width(500).Crop("Fill").Gravity("Face"),
                        Folder = folder
                    };

                    var result = await Task.Run(() => _cloudinary.Upload(uploadParams));
                    uploadResults.Add(result);
                }
            }

            return uploadResults;
        }
        public async Task<DeletionResult> DestroyFileAsync(string publicId)
        {
            var deletionParams = new DeletionParams(publicId);
            return await Task.Run(() => _cloudinary.Destroy(deletionParams));
        }
    }
}
