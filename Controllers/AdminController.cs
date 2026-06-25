using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vroom.Data;
using vroom.Models;

namespace vroom.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            var totalOrders = await _context.Orders.CountAsync();
            var totalUsers = await _userManager.Users.CountAsync();
            var pendingOrders = await _context.Orders
                .Where(o => o.Status == OrderStatus.Pending)
                .CountAsync();
            var totalRevenue = await _context.Orders
                .Where(o => o.Status != OrderStatus.Cancelled)
                .SumAsync(o => o.TotalPrice);

            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalUsers = totalUsers;
            ViewBag.PendingOrders = pendingOrders;
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TotalMotorcycles = await _context.Motorcycles.CountAsync();

            var recentOrders = await _context.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .Take(10)
                .ToListAsync();

            return View(recentOrders);
        }

        public async Task<IActionResult> Orders(OrderStatus? status = null)
        {
            var orders = _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Motorcycle)
                .AsQueryable();

            if (status.HasValue)
            {
                orders = orders.Where(o => o.Status == status.Value);
            }

            var result = await orders.OrderByDescending(o => o.OrderDate).ToListAsync();
            ViewBag.SelectedStatus = status;
            return View(result);
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Motorcycle)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int id, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            // Validate status transitions
            if (!IsValidStatusTransition(order.Status, status))
            {
                TempData["ErrorMessage"] = "მიუღებელი სტატუსის ცვლილება.";
                return RedirectToAction(nameof(OrderDetails), new { id });
            }

            order.Status = status;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"შეკვეთის სტატუსი განახლდა: {status}";
            return RedirectToAction(nameof(OrderDetails), new { id });
        }

        private bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            // Allow transitions
            return (currentStatus, newStatus) switch
            {
                // From Pending -> Processing, Cancelled
                (OrderStatus.Pending, OrderStatus.Processing) => true,
                (OrderStatus.Pending, OrderStatus.Cancelled) => true,
                // From Processing -> Shipped
                (OrderStatus.Processing, OrderStatus.Shipped) => true,
                // From Shipped -> Delivered
                (OrderStatus.Shipped, OrderStatus.Delivered) => true,
                // Prevent invalid transitions
                _ => false
            };
        }

        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> UserDetails(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var userOrders = await _context.Orders
                .Where(o => o.UserId == id)
                .ToListAsync();

            ViewBag.UserOrders = userOrders;
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                await _userManager.AddToRoleAsync(user, "Admin");
                user.IsAdmin = true;
                await _userManager.UpdateAsync(user);
            }

            TempData["SuccessMessage"] = "მომხმარებელი წარმატებით გახდა ადმინისტრატორი!";
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Admin");
                user.IsAdmin = false;
                await _userManager.UpdateAsync(user);
            }

            TempData["SuccessMessage"] = "ადმინისტრატორის უფლება წარმატებით ჩამოერთვა!";
            return RedirectToAction(nameof(Users));
        }

        // ───── Motorcycles CRUD ─────

        public async Task<IActionResult> Motorcycles()
        {
            var motorcycles = await _context.Motorcycles.OrderBy(m => m.Make).ThenBy(m => m.Model).ToListAsync();
            return View(motorcycles);
        }

        public IActionResult CreateMotorcycle()
        {
            return View(new Motorcycle { Make = "", Model = "", Category = "", ImageUrl = "", Engine = "", Horsepower = "", Torque = "", FuelCapacity = "", TopSpeed = "" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMotorcycle(Motorcycle motorcycle, string galleryImagesRaw)
        {
            // Parse newline-separated gallery URLs from textarea
            motorcycle.GalleryImages = ParseGalleryUrls(galleryImagesRaw);

            // Remove GalleryImages from ModelState so it doesn't block validation
            ModelState.Remove(nameof(Motorcycle.GalleryImages));

            if (!ModelState.IsValid)
                return View(motorcycle);

            _context.Motorcycles.Add(motorcycle);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"{motorcycle.Make} {motorcycle.Model} დამატებულია!";
            return RedirectToAction(nameof(Motorcycles));
        }

        public async Task<IActionResult> EditMotorcycle(int id)
        {
            var motorcycle = await _context.Motorcycles.FindAsync(id);
            if (motorcycle == null)
                return NotFound();

            ViewBag.GalleryImagesRaw = string.Join("\n", motorcycle.GalleryImages);
            return View(motorcycle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMotorcycle(int id, Motorcycle motorcycle, string galleryImagesRaw)
        {
            if (id != motorcycle.Id)
                return BadRequest();

            motorcycle.GalleryImages = ParseGalleryUrls(galleryImagesRaw);
            ModelState.Remove(nameof(Motorcycle.GalleryImages));

            if (!ModelState.IsValid)
            {
                ViewBag.GalleryImagesRaw = galleryImagesRaw;
                return View(motorcycle);
            }

            _context.Motorcycles.Update(motorcycle);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"{motorcycle.Make} {motorcycle.Model} განახლებულია!";
            return RedirectToAction(nameof(Motorcycles));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMotorcycle(int id)
        {
            var motorcycle = await _context.Motorcycles.FindAsync(id);
            if (motorcycle == null)
                return NotFound();

            _context.Motorcycles.Remove(motorcycle);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"{motorcycle.Make} {motorcycle.Model} წაიშალა!";
            return RedirectToAction(nameof(Motorcycles));
        }

        private static List<string> ParseGalleryUrls(string raw) =>
            (raw ?? "")
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(u => u.Trim())
                .Where(u => !string.IsNullOrEmpty(u))
                .ToList();
    }
}

