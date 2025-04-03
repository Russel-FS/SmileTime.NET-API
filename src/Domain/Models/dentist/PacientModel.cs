using System;

namespace SmileTimeNET_API.Domain.Entities.Dentist
{
    public class PacientModel
    {
        public string Id { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Name { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
    }
}
