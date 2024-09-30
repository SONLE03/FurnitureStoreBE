using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FurnitureStoreBE.Data;
using FurnitureStoreBE.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FurnitureStoreBE.Services.FileUploadService
{ 
    public class FileUploadServiceImp : IFileUploadService
    {
        private readonly Cloudinary _cloudinary;
        private readonly ApplicationDBContext _dbContext;
        public FileUploadServiceImp(IOptions<CloudinarySettings> config, ApplicationDBContext dBContext)
        {
            var account = new Account
            (
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(account);
            _dbContext = dBContext;
        }
        private async Task<ImageUploadResult> UploadImage(IFormFile file, string folder)
        {
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder
            };

            return await Task.Run(() => _cloudinary.Upload(uploadParams));
        }
        public async Task<ImageUploadResult> UploadFileAsync(IFormFile file, string folder)
        {
            return await UploadImage(file, folder);
        }
        public async Task<List<ImageUploadResult>> UploadFilesAsync(List<IFormFile> files, string folder)
        {
            var uploadResults = new List<ImageUploadResult>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    uploadResults.Add(await UploadImage(file,folder));
                }
            }
            return uploadResults;
        }
        
        public async Task<DeletionResult> DestroyFileByPublicIdAsync(string publicId)
        {
            var deletionParams = new DeletionParams(publicId);
            return await Task.Run(() => _cloudinary.Destroy(deletionParams));
        }
        public async Task<DeletionResult> DestroyFileByAssetIdAsync(Guid assetId)
        {
            var publicId = await _dbContext.Assets
                .Where(a => a.Id == assetId)
                .Select(a => a.CloudinaryId)
                .FirstOrDefaultAsync();
            var deletionParams = new DeletionParams(publicId);
            return await Task.Run(() => _cloudinary.Destroy(deletionParams));
        }
        public async Task<List<DeletionResult>> DestroyFilesByAssetIdsAsync(List<Guid> assetIds)
        {
            var publicIds = await _dbContext.Assets
                .Where(a => assetIds.Contains(a.Id))
                .Select(a => a.CloudinaryId)
                .ToListAsync();

            var deletionResults = new List<DeletionResult>();

            foreach (var publicId in publicIds)
            {
                var deletionParams = new DeletionParams(publicId);
                var result = await Task.Run(() => _cloudinary.Destroy(deletionParams));
                deletionResults.Add(result);
            }

            return deletionResults;
        }
    }
}
