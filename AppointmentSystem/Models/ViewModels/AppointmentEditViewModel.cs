using AppointmentSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentSystem.Models.ViewModels
{
    public class AppointmentEditViewModel
    {
        public Appointment Appointment { get; set; }
        public List<User> Doctor { get; set; }
        public List<string> AvailableTimeSlots { get; set; }
    }
}