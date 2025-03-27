using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmileTimeNET_API.src.Aplication.DTOs.chat
{
    public class AttachmentDTO
    {
        public int MessageId { get; set; }
        public int AttachmentId { get; set; }
        public string? FileUrl { get; set; } = string.Empty;
        public string? FileType { get; set; } = string.Empty;
        public string? FileName { get; set; } = string.Empty;
        public long? FileSize { get; set; }
        public DateTime UploadedAt { get; set; }

    }
}  
 