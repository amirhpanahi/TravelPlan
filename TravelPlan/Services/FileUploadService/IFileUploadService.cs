namespace TravelPlan.Services.FileUploadService
{
    public interface IFileUploadService
    {
        Task<string> UploadFileAsync(IFormFile file, string Name, string folderName, string? SubFolderName = null);
    }
}
