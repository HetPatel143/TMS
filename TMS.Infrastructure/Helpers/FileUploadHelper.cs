using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using TMS.Core.Exceptions;

namespace TMS.Infrastructure.Helpers
{
    public class FileUploadHelper
    {
        private readonly string _uploadFolder;
        private readonly long _maxFileSizeBytes;
        private readonly List<string> _allowedExtensions;

        public FileUploadHelper(IConfiguration configuration)
        {
            _uploadFolder = configuration["FileUpload:UploadFolder"] ?? "wwwroot/uploads";
            var maxSizeMB = int.Parse(configuration["FileUpload:MaxFileSizeMB"] ?? "10");
            _maxFileSizeBytes = maxSizeMB * 1024L * 1024L;
            _allowedExtensions = configuration.GetSection("FileUpload:AllowedExtensions")
                .Get<List<string>>() ?? new List<string> { ".pdf", ".jpg", ".jpeg", ".png", ".docx", ".xlsx" };
        }

        public async Task<(string FilePath, string FileName, string FileType)> SaveFileAsync(
            IFormFile file, string subFolder)
        {
            if (file == null || file.Length == 0)
                throw new ValidationException("No file was uploaded.");

            if (file.Length > _maxFileSizeBytes)
                throw new ValidationException($"File size exceeds the maximum allowed size of {_maxFileSizeBytes / (1024 * 1024)}MB.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
                throw new ValidationException($"File type '{extension}' is not allowed. Allowed types: {string.Join(", ", _allowedExtensions)}");

            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), _uploadFolder, subFolder);
            Directory.CreateDirectory(uploadPath);

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(uploadPath, uniqueFileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = Path.Combine("uploads", subFolder, uniqueFileName).Replace("\\", "/");

            return (relativePath, file.FileName, file.ContentType);
        }
    }
}
