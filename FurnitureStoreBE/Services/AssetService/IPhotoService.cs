namespace FurnitureStoreBE.Services.AssetService
{
    public interface IPhotoService
    {
        Task UploadImage<T>(IQueryable<T> query, Guid id, IFormFile file, string folder);
        Task UploadImages<T>(IQueryable<T> query, Guid id, List<IFormFile> files, string folder);
        Task UploadImage<T>(IQueryable<T> query, string id, IFormFile file, string folder);
        Task UploadImages<T>(IQueryable<T> query, string id, List<IFormFile> files, string folder);
    }
}
