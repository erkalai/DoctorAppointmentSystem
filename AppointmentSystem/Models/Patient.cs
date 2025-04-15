using System.ComponentModel.DataAnnotations;

namespace AppointmentSystem.Models
{
    public class Patient
    {
        [Key]
        public Guid PatientId { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Sex { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
    }
}
