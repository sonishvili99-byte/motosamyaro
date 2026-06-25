using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vroom.Data;
using vroom.Models;

namespace vroom.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var motorcycles = await _context.Motorcycles.ToListAsync();
            ViewData["AllMotorcycles"] = motorcycles;
            return View(motorcycles);
        }

        public async Task<IActionResult> Category(string name)
        {
            var allMotorcycles = await _context.Motorcycles.ToListAsync();
            var motorcycles = allMotorcycles.Where(m => m.Category == name).ToList();
            ViewData["Category"] = name;
            ViewData["AllMotorcycles"] = allMotorcycles;
            return View("Index", motorcycles);
        }

        public async Task<IActionResult> Search(string query)
        {
            var allMotorcycles = await _context.Motorcycles.ToListAsync();
            var motorcycles = string.IsNullOrWhiteSpace(query)
                ? allMotorcycles
                : allMotorcycles
                    .Where(m =>
                        m.Make.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                        m.Model.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                        m.Category.Contains(query, StringComparison.OrdinalIgnoreCase)
                    )
                    .ToList();

            ViewData["SearchQuery"] = query;
            ViewData["AllMotorcycles"] = allMotorcycles;
            return View("Index", motorcycles);
        }

        public IActionResult About()
        {
            return View();
        }

        public async Task<IActionResult> Gallery(string? category = null)
        {
            var allMotorcycles = await _context.Motorcycles.ToListAsync();
            var motorcycles = string.IsNullOrWhiteSpace(category)
                ? allMotorcycles
                : allMotorcycles.Where(m => m.Category == category).ToList();

            ViewData["AllMotorcycles"] = allMotorcycles;
            ViewData["SelectedCategory"] = category;
            return View(motorcycles);
        }
    }
}

