using System.Runtime.InteropServices;
using AppointmentSystem.Data;
using AppointmentSystem.Models;
using AppointmentSystem.Models.ViewModels;
using Microsoft.AspNetCore.Http.Connections;
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
        public async Task<IActionResult> GetAvailableSlots(Guid doctorId, DateTime date)
        {
            try
            {
                var bookedSlots = await _context.Appointments
                    .Where(a => a.UserId == doctorId &&
                                a.AppointmentDate.Date == date.Date &&
                                a.Status != "Cancelled" &&
                                a.Status != "Completed")
                    .Select(a => a.AppointmentTime.ToString(@"hh\:mm"))
                    .ToListAsync();

                var allSlots = GenerateTimeSlots();
                var availableSlots = allSlots.Except(bookedSlots).ToList();

                return Json(availableSlots);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error fetching available slots" });
            }
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
        public async Task<IActionResult> EditAppointment(Guid id)
        {
            try
            {
                var appointment = await _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                    .FirstOrDefaultAsync(a => a.AppointmentId == id);

                if (appointment == null)
                {
                    TempData["ErrorMessage"] = "Appointment not found";
                    return RedirectToAction("GetAllAppointments");
                }

                var doctors = await _context.Users
                    .Where(u => u.Role == "Doctor")
                    .OrderBy(u => u.FullName)
                    .ToListAsync();

                var availableSlots = await GetAvailableSlotsForDoctorAsync(appointment.UserId, appointment.AppointmentDate);

                // Include the current time slot even if it's already booked
                var currentTimeSlot = appointment.AppointmentTime.ToString(@"hh\:mm");
                if (!availableSlots.Contains(currentTimeSlot))
                {
                    availableSlots.Add(currentTimeSlot);
                    availableSlots.Sort();
                }

                var viewModel = new AppointmentEditViewModel
                {
                    Appointment = appointment,
                    Doctor = doctors,
                    AvailableTimeSlots = availableSlots
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while loading the appointment";
                return RedirectToAction("GetAllAppointments");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditAppointment(AppointmentEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Doctor = await _context.Users.Where(u => u.Role == "Doctor").ToListAsync();
                model.AvailableTimeSlots = await GetAvailableSlotsForDoctorAsync(
                    model.Appointment.UserId,
                    model.Appointment.AppointmentDate);
                return View(model);
            }

            try
            {
                var existingAppointment = await _context.Appointments.FindAsync(model.Appointment.AppointmentId);
                if (existingAppointment == null)
                {
                    return NotFound();
                }

                // Check slot availability
                var isSlotAvailable = await IsTimeSlotAvailable(
                    model.Appointment.UserId,
                    model.Appointment.AppointmentDate,
                    model.Appointment.AppointmentTime,
                    model.Appointment.AppointmentId
                );

                if (!isSlotAvailable)
                {
                    ModelState.AddModelError("Appointment.AppointmentTime", "This time slot is already booked");
                    model.Doctor = await _context.Users.Where(u => u.Role == "Doctor").ToListAsync();
                    model.AvailableTimeSlots = await GetAvailableSlotsForDoctorAsync(
                        model.Appointment.UserId,
                        model.Appointment.AppointmentDate);
                    return View(model);
                }

                // Update fields
                existingAppointment.UserId = model.Appointment.UserId;
                existingAppointment.AppointmentDate = model.Appointment.AppointmentDate;
                existingAppointment.AppointmentTime = model.Appointment.AppointmentTime;
                existingAppointment.Status = model.Appointment.Status;
                existingAppointment.Notes = model.Appointment.Notes;

                _context.Update(existingAppointment);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Appointment updated successfully!";
                return RedirectToAction("GetAllAppointments");
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error updating appointment");
                TempData["ErrorMessage"] = "An error occurred while updating the appointment";

                model.Doctor = await _context.Users.Where(u => u.Role == "Doctor").ToListAsync();
                model.AvailableTimeSlots = await GetAvailableSlotsForDoctorAsync(
                    model.Appointment.UserId,
                    model.Appointment.AppointmentDate);
                return View(model);
            }
        }

        private async Task<bool> IsTimeSlotAvailable(Guid doctorId, DateTime date, TimeSpan time, Guid currentAppointmentId)
        {
            return !await _context.Appointments
                .AnyAsync(a => a.UserId == doctorId &&
                              a.AppointmentId != currentAppointmentId &&
                              a.AppointmentDate.Date == date.Date &&
                              a.AppointmentTime == time &&
                              a.Status != "Cancelled");
        }

        private bool AppointmentExists(Guid id)
        {
            return _context.Appointments.Any(e => e.AppointmentId == id);
        }

        private async Task<List<string>> GetAvailableSlotsForDoctorAsync(Guid doctorId, DateTime date)
        {
            var bookedSlots = await _context.Appointments
                .Where(a => a.UserId == doctorId &&
                           a.AppointmentDate.Date == date.Date &&
                           a.Status != "Cancelled" &&
                           a.Status != "Completed")
                .Select(a => a.AppointmentTime.ToString(@"hh\:mm"))
                .ToListAsync();

            var allSlots = GenerateTimeSlots();

            return allSlots.Except(bookedSlots).ToList();
        }

        private List<string> GenerateTimeSlots()
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
            return allSlots;
        }
    }
}
