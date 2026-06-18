namespace TMS.Core.Entities
{
    public class RequestAttachment
    {
        public int AttachmentId { get; set; }
        public int RequestId { get; set; }
        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public string? FileType { get; set; }
        public int UploadedByUserId { get; set; }
        public DateTime UploadedAt { get; set; }

        public virtual Request Request { get; set; } = null!;
        public virtual User UploadedByUser { get; set; } = null!;
    }
}
