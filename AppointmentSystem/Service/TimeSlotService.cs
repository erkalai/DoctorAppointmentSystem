using AppointmentSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppointmentSystem.Service
{
    public class TimeSlotService
    {
        private readonly ApplicationDbContext _context;

        public TimeSlotService(ApplicationDbContext context)
        {
            this._context = context;
        }

        public async Task<List<TimeSpan>> GetAvailableSlots(string doctorId, DateTime date)
        {
            var doctor = await _context.Users.FindAsync(doctorId);
            if (doctor == null || doctor.Role != "Doctor")
                return new List<TimeSpan>();

            var dayOfWeek = date.DayOfWeek.ToString();
            if (doctor.UnavilableDays?.Contains(dayOfWeek) == true)
            {
                return new List<TimeSpan>();
            }

            Guid doctorGuid = Guid.Parse(doctorId);

            var appointments = await _context.Appointments
                .Where(a => a.UserId == doctorGuid &&
                       a.AppointmentDate.Date == date.Date &&
                       a.Status != "Cancelled")
            .OrderBy(a => a.AppointmentTime)
            .ToListAsync();

            var allSlots = GenerateTimeSlots
                (
                doctor.WorkDayStart ?? new TimeSpan(9, 0, 0),
                doctor.WorkDayEnd ?? new TimeSpan(17, 0, 0),
                doctor.AppointmentDuration ?? 20
                );

            var bookedSlots = appointments.Select(a => a.AppointmentTime).ToList();
            return allSlots.Except(bookedSlots).ToList();
        }

        private List<TimeSpan> GenerateTimeSlots(TimeSpan start, TimeSpan end, int duration)
        {
            var slots = new List<TimeSpan>();
            var current = start;

            while (current + TimeSpan.FromMinutes(duration)<= end)
            {
                slots.Add(current);
                current = current + TimeSpan.FromMinutes(duration);
            }
            return slots;
        }
    }
}
