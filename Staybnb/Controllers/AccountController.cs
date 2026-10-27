using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Staybnb.Data;
using Staybnb.Models;
using Staybnb.Models.ViewModels;

namespace Staybnb.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                CreatedAt = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // ALL new registrants → Guest role
                await _userManager.AddToRoleAsync(user, "Guest");

                _context.ActivityLogs.Add(new ActivityLog
                {
                    UserId = user.Id,
                    Action = "New user registered",
                    ActivityType = ActivityType.Registration
                });
                await _context.SaveChangesAsync();

                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    _context.ActivityLogs.Add(new ActivityLog
                    {
                        UserId = user.Id,
                        Action = "User logged in",
                        ActivityType = ActivityType.Login
                    });
                    await _context.SaveChangesAsync();

                    if (await _userManager.IsInRoleAsync(user, "SuperAdmin") ||
                        await _userManager.IsInRoleAsync(user, "Admin"))
                        return RedirectToAction("Index", "AdminDashboard");

                    if (await _userManager.IsInRoleAsync(user, "Host"))
                        return RedirectToAction("Index", "HostDashboard");

                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                _context.ActivityLogs.Add(new ActivityLog
                {
                    UserId = user.Id,
                    Action = "User logged out",
                    ActivityType = ActivityType.Logout
                });
                await _context.SaveChangesAsync();
            }

            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}