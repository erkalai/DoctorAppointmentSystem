namespace AppointmentSystem.Models.Dtos
{
    public class UpdateUser
    {
            public Guid UserId { get; set; }
            public string? Password { get; set; }
            public string Role { get; set; } // "Admin", "Doctor", "Receptionist"
            public string FullName { get; set; }
            public string? Email { get; set; }
            public string Phone { get; set; }


            //Doctor _ Fields
            public string? Specialization { get; set; }
            public string? Qualifications { get; set; }
            public TimeSpan? WorkDayStart { get; set; }
            public TimeSpan? WorkDayEnd { get; set; }
            public int? AppointmentDuration { get; set; }
            public string? UnavilableDays { get; set; }
            public string? AvailableDays { get; set; }
    }
}
