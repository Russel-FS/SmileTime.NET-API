using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmileTimeNET_API.src.Aplication.DTOs.chat
{
    public class AttachmentDTO
    {
        public int AttachmentId { get; set; }
        public string FileUrl { get; set; }
        public string FileType { get; set; }
    }
}