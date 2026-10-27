using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Staybnb.Data;
using Staybnb.Models;
using Staybnb.Services;

namespace Staybnb.Controllers
{
    [Authorize(Roles = "Host")]
    public class HostDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly BookingService _bookingService;

        public HostDashboardController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            BookingService bookingService)
        {
            _context = context;
            _userManager = userManager;
            _bookingService = bookingService;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var properties = await _context.HostProperties
                .Where(p => p.HostId == user.Id)
                .Include(p => p.Images)
                .ToListAsync();

            var bookings = await _context.Bookings
                .Include(b => b.Property)
                .Include(b => b.Guest)
                .Where(b => b.Property!.HostId == user.Id)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            ViewBag.TotalProperties = properties.Count;
            ViewBag.ActiveProperties = properties.Count(p => p.IsActive);
            ViewBag.TotalBookings = bookings.Count;
            ViewBag.TotalEarnings = bookings
                .Where(b => b.Status == BookingStatus.Approved || b.Status == BookingStatus.CheckedIn)
                .Sum(b => b.TotalPrice);

            return View(bookings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveBooking(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            await _bookingService.ApproveBookingAsync(id, user.Id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectBooking(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            await _bookingService.RejectBookingAsync(id, user.Id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePropertyStatus(int id)
        {
            var property = await _context.HostProperties.FindAsync(id);
            if (property == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (property.HostId != user?.Id) return Forbid();

            property.IsActive = !property.IsActive;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}