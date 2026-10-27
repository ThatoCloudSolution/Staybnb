using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Staybnb.Data;
using Staybnb.Models;
using Staybnb.Services;

namespace Staybnb.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HostApplicationService _hostAppService;

        public AdminDashboardController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            HostApplicationService hostAppService)
        {
            _context = context;
            _userManager = userManager;
            _hostAppService = hostAppService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.PendingApplications = await _context.HostApplications
                .CountAsync(a => a.Status == ApplicationStatus.Pending);
            ViewBag.ActiveProperties = await _context.HostProperties
                .CountAsync(p => p.IsActive);
            ViewBag.TotalBookings = await _context.Bookings.CountAsync();
            ViewBag.ApprovedHosts = (await _userManager.GetUsersInRoleAsync("Host")).Count;
            ViewBag.PlatformRevenue = await _context.Bookings
                .Where(b => b.Status == BookingStatus.Approved || b.Status == BookingStatus.CheckedIn)
                .SumAsync(b => (decimal?)b.ServiceFee) ?? 0m;

            ViewBag.RecentActivity = await _context.ActivityLogs
                .Include(a => a.User)
                .OrderByDescending(a => a.CreatedAt)
                .Take(10)
                .ToListAsync();

            return View();
        }

        public async Task<IActionResult> Applications()
        {
            var apps = await _context.HostApplications
                .Include(a => a.ApplicationUser)
                .Include(a => a.Property)
                .ThenInclude(p => p!.Images)
                .OrderByDescending(a => a.AppliedAt)
                .ToListAsync();

            return View(apps);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            await _hostAppService.ApproveApplicationAsync(id, user.Id);
            return RedirectToAction(nameof(Applications));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string notes)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            await _hostAppService.RejectApplicationAsync(id, user.Id, notes ?? "No reason provided");
            return RedirectToAction(nameof(Applications));
        }

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Users()
        {
            var users = await _context.Users.ToListAsync();
            var userRoles = new Dictionary<string, IList<string>>();

            foreach (var u in users)
                userRoles[u.Id] = await _userManager.GetRolesAsync(u);

            ViewBag.UserRoles = userRoles;
            return View(users);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PromoteToAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            if (await _userManager.IsInRoleAsync(user, "Guest"))
                await _userManager.RemoveFromRoleAsync(user, "Guest");

            if (!await _userManager.IsInRoleAsync(user, "Admin"))
                await _userManager.AddToRoleAsync(user, "Admin");

            var admin = await _userManager.GetUserAsync(User);
            _context.ActivityLogs.Add(new ActivityLog
            {
                UserId = admin!.Id,
                Action = $"Promoted {user.Email} to Admin",
                ActivityType = ActivityType.RoleChange
            });
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Users));
        }

        public async Task<IActionResult> ActivityLogs()
        {
            var logs = await _context.ActivityLogs
                .Include(l => l.User)
                .OrderByDescending(l => l.CreatedAt)
                .Take(200)
                .ToListAsync();
            return View(logs);
        }
    }
}