using AppointmentSystem.Data;
using AppointmentSystem.Models;
using AppointmentSystem.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AppointmentSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger;
        public AccountController(ApplicationDbContext dbContext, ILogger<AccountController> logger)
        {
            this._context = dbContext;
            this._logger = logger;
        }

        // GET: Account/Register
        public IActionResult Register()
        {
            if (!_context.Users.Any())
            {
                return View();
            }

            if (HttpContext.Session.GetString("Role") == "Admin")
            {
                return View();
            }

            return RedirectToAction("Login");
        }

        // POST: Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any())
                {
                    user.Role = "Admin";
                }
                else if (HttpContext.Session.GetString("Role") != "Admin")
                {
                    return RedirectToAction("Login");
                }

                // Check if the email already exists in the database
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email Already Exists");
                    return View(user);
                }

                _context.Add(user);
                await _context.SaveChangesAsync();

                // Check if the user is an admin

                if (!_context.Users.Any(u => u.UserId != user.UserId))
                {
                    HttpContext.Session.SetString("UserId", user.UserId.ToString());
                    HttpContext.Session.SetString("Role", user.Role);
                    HttpContext.Session.SetString("Email", user.Email);
                    return RedirectToAction("AdminDashboard", "Home");
                }

                return RedirectToAction("ManageUsers","Home");
            }
            return View(user);
        }

        // GET: Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
            if (user != null)
            {
                HttpContext.Session.SetString("UserId", user.UserId.ToString());
                HttpContext.Session.SetString("Role", user.Role);
                HttpContext.Session.SetString("Email", user.Email);
                return RedirectToAction("DashBoard", "Home");
            }
            ViewBag.ErrorMessage = "Invalid email or password.";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> UpdateUser(string id)
        {
            Guid userIdGuid = new Guid(id);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userIdGuid);

            var dto = new UpdateUser
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Phone = user.Phone,
                Email = user.Email,
                Password = user.Password,
                Role = user.Role,
                Specialization = user.Specialization,
                Qualifications = user.Qualifications,
                WorkDayStart = user.WorkDayStart,
                WorkDayEnd = user.WorkDayEnd,
                AppointmentDuration = user.AppointmentDuration,
                UnavilableDays = user.UnavilableDays,
                AvailableDays = user.AvailableDays
            };

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(UpdateUser user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _context.Users.FindAsync(user.UserId);
                if (existingUser != null)
                {
                    existingUser.FullName = user.FullName;
                    existingUser.Email = user.Email;
                    existingUser.Phone = user.Phone;
                    existingUser.Password = user.Password;
                    existingUser.Role = user.Role;
                    existingUser.Specialization = user.Specialization;
                    existingUser.Qualifications = user.Qualifications;
                    existingUser.WorkDayStart = user.WorkDayStart;
                    existingUser.WorkDayEnd = user.WorkDayEnd;
                    existingUser.AppointmentDuration = user.AppointmentDuration;
                    existingUser.UnavilableDays = user.UnavilableDays;
                    existingUser.AvailableDays = user.AvailableDays;
                    await _context.SaveChangesAsync();
                    return RedirectToAction("ManageUsers", "Home");
                } else
                {
                    ModelState.AddModelError("", "User not found.");
                }
            }
            return View(user);
        }
        // GET: Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
