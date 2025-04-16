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

        
    }
}
