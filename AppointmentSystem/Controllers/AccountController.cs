using AppointmentSystem.Data;
using AppointmentSystem.Models;
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

        // GET: Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
