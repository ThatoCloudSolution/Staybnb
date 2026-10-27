using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Staybnb.Data;

namespace Staybnb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? city, string? type)
        {
            var query = _context.HostProperties
                .Include(p => p.Images)
                .Where(p => p.IsActive)
                .AsQueryable();

            if (!string.IsNullOrEmpty(city))
                query = query.Where(p => p.City.Contains(city));

            if (!string.IsNullOrEmpty(type))
                query = query.Where(p => p.PropertyType == type);

            ViewBag.Cities = await _context.HostProperties
                .Where(p => p.IsActive)
                .Select(p => p.City).Distinct().ToListAsync();

            return View(await query.ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var property = await _context.HostProperties
                .Include(p => p.Images)
                .Include(p => p.Host)
                .Include(p => p.Reviews)
                .Include(p => p.Amenities)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (property == null) return NotFound();
            return View(property);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View();
    }
}