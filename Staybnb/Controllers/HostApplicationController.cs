using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Staybnb.Data;
using Staybnb.Models;
using Staybnb.Models.ViewModels;

namespace Staybnb.Controllers
{
    [Authorize(Roles = "Guest")]
    public class HostApplicationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public HostApplicationController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        [HttpGet]
        public IActionResult Apply() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(HostApplicationViewModel model)
        {
            if (model.Image == null || model.Image.Length == 0)
                ModelState.AddModelError("Image", "At least one property image is required.");

            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(model.Image!.FileName);
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsFolder);
            var filePath = Path.Combine(uploadsFolder, uniqueName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.Image.CopyToAsync(stream);
            }

            var property = new HostProperty
            {
                Title = model.Title,
                Description = model.Description,
                PricePerNight = model.PricePerNight,
                Address = model.Address,
                City = model.City,
                PropertyType = model.PropertyType,
                Bedrooms = model.Bedrooms,
                Bathrooms = model.Bathrooms,
                MaxGuests = model.MaxGuests,
                HostId = user.Id,
                IsActive = false
            };
            _context.HostProperties.Add(property);
            await _context.SaveChangesAsync();

            _context.PropertyImages.Add(new PropertyImage
            {
                PropertyId = property.Id,
                ImageUrl = "/uploads/" + uniqueName
            });

            _context.HostApplications.Add(new HostApplication
            {
                ApplicationUserId = user.Id,
                PropertyId = property.Id,
                Status = ApplicationStatus.Pending
            });

            _context.ActivityLogs.Add(new ActivityLog
            {
                UserId = user.Id,
                Action = $"Submitted host application for '{property.Title}'",
                ActivityType = ActivityType.ApplicationSubmitted
            });

            await _context.SaveChangesAsync();

            return RedirectToAction("Status");
        }

        public async Task<IActionResult> Status()
        {
            var user = await _userManager.GetUserAsync(User);
            var applications = await _context.HostApplications
                .Include(a => a.Property)
                .ThenInclude(p => p!.Images)
                .Where(a => a.ApplicationUserId == user!.Id)
                .OrderByDescending(a => a.AppliedAt)
                .ToListAsync();

            return View(applications);
        }
    }
}