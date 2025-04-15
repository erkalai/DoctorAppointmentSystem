using System.ComponentModel.DataAnnotations;

namespace AppointmentSystem.Models
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // "Admin", "Doctor", "Receptionist"
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }


        //Doctor _ Fields
        public string? Specialization { get; set; }
        public string? Qualifications { get; set; }
        public string? AvailableDays { get; set; }
        public TimeSpan? AvailableFrom { get; set; }
        public TimeSpan? AvailableTo { get; set; }
    }
}
