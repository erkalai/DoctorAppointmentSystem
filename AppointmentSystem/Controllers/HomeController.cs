using System.Diagnostics;
using AppointmentSystem.Data;
using AppointmentSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppointmentSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        //GET: Home/DashBoard
        public async Task<IActionResult> Dashboard()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var userId = HttpContext.Session.GetString("UserId");
            var role = HttpContext.Session.GetString("Role");

            Guid doctorGuid = Guid.Parse(userId);

            switch(role)
            {
                case "Admin":
                    ViewBag.UserCount = await _context.Users.CountAsync();
                    ViewBag.PatientsCount = await _context.Patients.CountAsync();
                    ViewBag.AppointmentsCount = await _context.Appointments.CountAsync();
                    return View("AdminDashboard");
                case "Doctor":
                    var doctorAppointments = await _context.Appointments
                    .Include(a => a.Patient)
                    .Where(a => a.UserId == doctorGuid && a.Status == "Scheduled")
                    .OrderBy(a => a.AppointmentDate)
                    .ThenBy(a => a.AppointmentTime)
                    .ToListAsync();
                    return View("DoctorDashboard", doctorAppointments);
                case "Receptionist":
                    var upcommingAppointments = await _context.Appointments
                        .Include(a => a.Patient)
                        .Include(a => a.Doctor)
                        .Where(a => a.AppointmentDate >= DateTime.Today && a.Status == "Scheduled")
                        .OrderBy(a => a.AppointmentDate)
                        .ThenBy(a => a.AppointmentTime)
                        .ToListAsync();
                    return View("ReceptionistDashboard", upcommingAppointments);
                default:
                    return RedirectToAction("Login","Account");
            }
        }
        //[AdminOnly]
        public async Task<IActionResult> ManageUsers()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        //[AdminOnly]
        public IActionResult CreateUser()
        {
            return View();
        }

        //[AdminOnly]
        [HttpPost]
        public async Task<IActionResult> CreateUser(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("ManageUsers");
            }
            return View(user);
        }
    }
}
