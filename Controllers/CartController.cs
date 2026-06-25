using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vroom.Data;
using vroom.Models;

namespace vroom.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var cartItems = await _context.CartItems
                .Where(c => c.UserId == user.Id)
                .Include(c => c.Motorcycle)
                .ToListAsync();

            ViewBag.TotalPrice = cartItems.Sum(c => c.Motorcycle?.Price * c.Quantity ?? 0);
            ViewBag.TotalItems = cartItems.Sum(c => c.Quantity);

            return View(cartItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int motorcycleId, int quantity = 1)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            // Try to find motorcycle in database first
            var motorcycle = await _context.Motorcycles.FindAsync(motorcycleId);
            
            // If not in database, create a temporary one from in-memory list
            if (motorcycle == null)
            {
                // This allows adding to cart even if motorcycles are only in-memory
                return RedirectToAction("Index", "Home");
            }

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == user.Id && c.MotorcycleId == motorcycleId);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cartItem = new CartItem
                {
                    UserId = user.Id,
                    MotorcycleId = motorcycleId,
                    Quantity = quantity
                };
                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();

            
            TempData["SuccessMessage"] = $"{motorcycle.Make} {motorcycle.Model} წარმატებით დაემატა კალათაში!";
            return RedirectToAction("Index", "Cart");
        }

        // Optional GET helper for development: /Cart/AddToCart?motorcycleId=1
        // Accessible via query string for direct links
        [Authorize]
        public async Task<IActionResult> AddToCartGet(int motorcycleId, int quantity = 1)
        {
            if (motorcycleId <= 0)
            {
                TempData["ErrorMessage"] = "მოგიწევთ აირჩიოთ პროდუქტი დამატებისთვის.";
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var motorcycle = await _context.Motorcycles.FindAsync(motorcycleId);
            if (motorcycle == null)
                return NotFound();

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == user.Id && c.MotorcycleId == motorcycleId);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cartItem = new CartItem
                {
                    UserId = user.Id,
                    MotorcycleId = motorcycleId,
                    Quantity = quantity
                };
                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"{motorcycle.Make} {motorcycle.Model} წარმატებით დაემატა კალათაში!";
            return RedirectToAction("Index", "Cart");
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == user.Id);

            if (cartItem == null)
                return NotFound();

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

           
            TempData["SuccessMessage"] = "წარმატებით წაიშალა კალათიდან.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == user.Id);

            if (cartItem == null)
                return NotFound();

            if (quantity <= 0)
            {
                _context.CartItems.Remove(cartItem);
            }
            else
            {
                cartItem.Quantity = quantity;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var cartItems = await _context.CartItems
                .Where(c => c.UserId == user.Id)
                .Include(c => c.Motorcycle)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["ErrorMessage"] = "თქვენი კალათა ცარიელია.";
                return RedirectToAction("Index");
            }

            // Create order
            var order = new Order
            {
                UserId = user.Id,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                TotalPrice = cartItems.Sum(c => c.Motorcycle?.Price * c.Quantity ?? 0)
            };

            // Add order items
            foreach (var cartItem in cartItems)
            {
                order.Items.Add(new OrderItem
                {
                    MotorcycleId = cartItem.MotorcycleId,
                    Quantity = cartItem.Quantity,
                    PriceAtPurchase = cartItem.Motorcycle?.Price ?? 0
                });
            }

            _context.Orders.Add(order);

            // Clear cart
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            // success message after order creation
            TempData["SuccessMessage"] = $"შეკვეთა #{order.Id} წარმატებით შეინახა!";
            return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
        }

        public async Task<IActionResult> OrderConfirmation(int orderId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var order = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(oi => oi.Motorcycle)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == user.Id);

            if (order == null)
                return NotFound();

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == user.Id);

            if (order == null)
                return NotFound();

            // Only allow cancellation for Pending orders
            if (order.Status != OrderStatus.Pending)
            {
                TempData["ErrorMessage"] = "მხოლოდ პროცესირების პერიოდში შეიძლება შეკვეთის გაუქმება.";
                return RedirectToAction("OrderDetails", "Account", new { id = orderId });
            }

            order.Status = OrderStatus.Cancelled;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"შეკვეთა #{orderId} წარმატებით გაუქმდა.";
            return RedirectToAction("MyOrders", "Account");
        }
    }
}

