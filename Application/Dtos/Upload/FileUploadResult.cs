using Application.Dtos.Response;

namespace Application.Dtos.Upload
{
    public class FileUploadResult
    {
        public BaseApiResponse Response { get; set; } = null!;
        public string? StoredFileName { get; set; }
        public string? OriginalFileName { get; set; }
    }
}
