using AppointmentSystem.Data;
using AppointmentSystem.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppointmentSystem.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AppointmentController(ApplicationDbContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public async Task<IActionResult> CreateAppointment  ()
        {
           ViewBag.Doctors = await _context.Users.Where(u => u.Role == "Doctor").ToListAsync();
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> GetAvailableSlots(string doctorId, DateTime date)
        {
            var startHour = 9;
            var endHour = 17;
            var interval = 20;
            var allSlots = new List<string>();
            for (var hour = startHour; hour < endHour; hour++)
            {
                for (int minutes = 0; minutes < 60; minutes += interval)
                {
                    allSlots.Add($"{hour:00}:{minutes:00}");
                }
            }

            var bookedSlots = await _context.Appointments
                .Where(a => a.UserId.ToString() == doctorId && a.AppointmentDate.Date == date.Date && a.Status != "Cancelled")
                .Select(a => a.AppointmentTime.ToString(@"hh\:mm"))
                .ToListAsync();


            return Json(allSlots.Except(bookedSlots).ToList());
        }



        [HttpPost]
        public async Task<IActionResult> CreateAppointment(Appointment appointment)
        {
            var patientExists = await _context.Patients
                .AnyAsync(p => p.PatientId == appointment.PatientId);

            if (!patientExists)
            {
                ModelState.AddModelError("PatientId", "Invalid patient selected");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    appointment.AppointmentId = Guid.NewGuid();
                    appointment.Status = "Scheduled";

                    _context.Add(appointment);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Appointment booked successfully!";
                    return RedirectToAction("CreateAppointment");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error saving appointment: " + ex.Message);
                }
            }

            // If we got here, something failed
            ViewBag.Doctors = await _context.Users.Where(u => u.Role == "Doctor").ToListAsync();
            return View(appointment);
        }

        [HttpGet]
        public async Task<IActionResult> GetPatientByMobile(string mobile)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Phone == mobile);


            return Json(new
            {
                success = true,
                patient = new
                {
                    id = patient.PatientId.ToString(),
                    fullName = patient.FullName,
                    phone = patient.Phone
                }
            });

            //return patient == null ? NotFound() : Json(patient);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAppointments()
        {
            var upcommingAppointments = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .Where(a => a.AppointmentDate >= DateTime.Today)
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .ToListAsync();
            return View(upcommingAppointments);
        }

        [HttpGet]
        public async Task<IActionResult> AppointmentDetails(string id)
        {
            if (!Guid.TryParse(id, out Guid appointmentGuid))
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentGuid);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        [HttpGet]
        public async Task<IActionResult> EditAppointment(string id)
        {
            ViewBag.Doctors = await _context.Users.Where(u => u.Role == "Doctor").ToListAsync();

            if (!Guid.TryParse(id, out Guid appointmentGuid))
            {
                return NotFound();
            }
            var appointment = await _context.Appointments
                   .Include(a => a.Patient) 
                   .Include(a => a.Doctor)
                   .FirstOrDefaultAsync(a => a.AppointmentId == appointmentGuid);
            if(appointment == null)
            {
                return NotFound();
            }
            ViewBag.Patient = appointment.Patient.FullName;
            ViewBag.PatientPhone = appointment.Patient.Phone;
            return View(appointment);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAppointment(Appointment appointment)
        {
            var existingAppointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointment.AppointmentId);

            if (existingAppointment == null)
            {
                return NotFound();
            }

            existingAppointment.UserId = appointment.UserId;
            existingAppointment.AppointmentDate = appointment.AppointmentDate;
            existingAppointment.AppointmentTime = appointment.AppointmentTime;
            existingAppointment.Status = "Scheduled";

            _context.Appointments.Update(existingAppointment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Dashboard", "Home");
        }

    }
}
