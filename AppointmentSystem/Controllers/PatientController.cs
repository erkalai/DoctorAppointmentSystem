using AppointmentSystem.Data;
using AppointmentSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppointmentSystem.Controllers
{
    public class PatientController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PatientController(ApplicationDbContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var patient = await _context.Patients.ToListAsync();
            return View(patient);
        }

        [HttpGet]
        public IActionResult CreatePatient()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePatient(Patient patient)
        {
            ModelState.Remove("Phone");
            ModelState.Remove("Email");
            ModelState.Remove("Address");
            ModelState.Remove("Sex");
            ModelState.Remove("DateOfBirth");

            if (ModelState.IsValid)
            {
                _context.Add(patient);
                await _context.SaveChangesAsync();
                return RedirectToAction("CreatePatient", "Patient");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PatientDetails(string id)
        {
            Guid patientId = new Guid(id);

            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == patientId);

            return View(patient);
        }

        [HttpGet]
        public async Task<IActionResult> UpdatePatient(string id)
        {
            Guid guidId = new Guid(id);
            var existPatient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == guidId);
            if(existPatient == null)
            {
                return BadRequest("Invalid");
            }
            return View(existPatient);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePatient(Patient patient)
        {
            var existPatient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == patient.PatientId);

            if (existPatient != null)
            {
                existPatient.FullName = patient.FullName;
                existPatient.Address = patient.Address;
                existPatient.DateOfBirth = patient.DateOfBirth;
                existPatient.Email = patient.Email;
                existPatient.Phone = patient.Phone;
                existPatient.Sex = patient.Sex;
                existPatient.RegistrationDate = patient.RegistrationDate;

                _context.Update(existPatient);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> DeletePatient(string id)
        {
            Guid patientId = new Guid(id);
            var existPatient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == patientId);

            return View(existPatient);
        }

        [HttpPost]
        //[ActionName("Delete")]
        public async Task<IActionResult> DeletePatientConfirmed(string id)
        {
            Guid patientGuid = new Guid(id);  
            var existPatient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == patientGuid);
            if(existPatient != null)
            {
                _context.Patients.Remove(existPatient);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

          return RedirectToAction("Index");
        }
    }
}
