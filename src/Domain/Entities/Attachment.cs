using System;

namespace SmileTimeNET_API.Models
{
    public class Attachment
    {
        public int AttachmentId { get; set; }
        public int MessageId { get; set; }
        public string? FileUrl { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }

        // Navegacion
        public Message? Message { get; set; }
    }
}
