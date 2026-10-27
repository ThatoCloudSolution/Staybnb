using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Staybnb.Data;
using Staybnb.Models;
using Staybnb.Models.ViewModels;
using Staybnb.Services;

namespace Staybnb.Controllers
{
    [Authorize(Roles = "Guest")]
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly BookingService _bookingService;

        public BookingController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            BookingService bookingService)
        {
            _context = context;
            _userManager = userManager;
            _bookingService = bookingService;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int propertyId)
        {
            var property = await _context.HostProperties.FindAsync(propertyId);
            if (property == null) return NotFound();

            var vm = new BookingViewModel
            {
                PropertyId = property.Id,
                PropertyTitle = property.Title,
                PricePerNight = property.PricePerNight
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            try
            {
                await _bookingService.CreateBookingAsync(
                    user.Id, model.PropertyId, model.CheckInDate,
                    model.CheckOutDate, model.NumberOfGuests);

                return RedirectToAction(nameof(MyBookings));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        public async Task<IActionResult> MyBookings()
        {
            var user = await _userManager.GetUserAsync(User);
            var bookings = await _context.Bookings
                .Include(b => b.Property)
                .ThenInclude(p => p!.Images)
                .Where(b => b.GuestId == user!.Id)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return View(bookings);
        }
    }
}