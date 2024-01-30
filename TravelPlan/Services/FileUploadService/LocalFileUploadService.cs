namespace TravelPlan.Services.FileUploadService
{
    public class LocalFileUploadService : IFileUploadService
    {
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
        public LocalFileUploadService(Microsoft.AspNetCore.Hosting.IHostingEnvironment environment)
        {
            _environment = environment;
        }
        public async Task<string> UploadFileAsync(IFormFile file, string Name, string folderName, string? SubFolderName)
        {
            string ReplaceDate = DateTime.Now.ToString();
            ReplaceDate = ReplaceDate.Replace("/", "-").Replace(":", "-").Replace(" ","$");
            var FileNameExtention = "." + file.FileName.Split('.')[1];
            var fileName = ReplaceDate + "$" + Name.Trim() +FileNameExtention;
            if (folderName==null)
            {
                fileName = "";
            }
            else
            {
                if (!Directory.Exists(_environment.ContentRootPath + @"wwwroot\Media\" + folderName + (SubFolderName != null ? @"\" + SubFolderName : null)))
                {
                    Directory.CreateDirectory(_environment.ContentRootPath + @"wwwroot\Media\" + folderName + (SubFolderName != null ? @"\" + SubFolderName : null));
                }

                var FilePath = Path.Combine(_environment.ContentRootPath, @"wwwroot\Media\" + folderName + (SubFolderName != null ? @"\" + SubFolderName : null), fileName);

                var StreamFile = new FileStream(FilePath, FileMode.Create);
                await file.CopyToAsync(StreamFile);
            }
            
            return fileName;
        }
    }
}
