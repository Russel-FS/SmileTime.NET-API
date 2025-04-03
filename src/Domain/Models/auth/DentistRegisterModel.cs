namespace SmileTimeNET_API.Models
{
    public class DentistRegisterModel : RegisterModel
    {
        public string? Specialization { get; set; }
        public bool Active { get; set; } = true;

        public DentistRegisterModel()
        {
            Role = "Dentist";
        }
    }
}
