using Application.Dtos.Response;
using Application.Dtos.Upload;
using Microsoft.AspNetCore.Http;

namespace Application.Helpers.Upload
{
    public static class DocumentSettings
    {
        private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
        private const long MaxFileSizeBytes = 5 * 1024 * 1024;

        public static FileUploadResult UpdateProfilePhoto(IFormFile file, string? oldFileName)
        {
            if (file is null || file.Length == 0)
                return new FileUploadResult { Response = BaseApiResponse.Fail(400, "Photo is required.") };
            if (file.Length > MaxFileSizeBytes)
                return new FileUploadResult { Response = BaseApiResponse.Fail(400, "Photo must be 5MB or smaller.") };

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension) || !file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                return new FileUploadResult { Response = BaseApiResponse.Fail(400, "Use a JPG, PNG, WebP, or JPEG image.") };

            var folder = Path.Combine(Directory.GetCurrentDirectory(), "..", "Pic", "employees");
            Directory.CreateDirectory(folder);
            var fileName = $"{Guid.NewGuid():N}{extension}";
            using (var stream = new FileStream(Path.Combine(folder, fileName), FileMode.Create))
                file.CopyTo(stream);

            if (!string.IsNullOrWhiteSpace(oldFileName))
            {
                var oldPath = Path.Combine(folder, oldFileName);
                if (File.Exists(oldPath)) File.Delete(oldPath);
            }

            return new FileUploadResult
            {
                Response = BaseApiResponse.Success(200, "Profile photo uploaded successfully."),
                StoredFileName = fileName,
                OriginalFileName = file.FileName
            };
        }
    }
}
