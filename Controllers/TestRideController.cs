using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using vroom.Data;
using vroom.Models;

namespace vroom.Controllers
{
    [Authorize]
    public class TestRideController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TestRideController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> BookTestRide(string phoneNumber)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Validate phone number
            if (string.IsNullOrWhiteSpace(phoneNumber) || !phoneNumber.All(char.IsDigit) || phoneNumber.Length < 9)
            {
                TempData["Error"] = "მობილურის ნომერი არასწორია. გთხოვთ შეიყვანოთ მხოლოდ ციფრები (მინიმუმ 9 ციფრი).";
                return RedirectToAction("Index", "Home");
            }

            // Check if terms were agreed (this is handled in the form validation)
            var testRide = new TestRide
            {
                UserId = user.Id,
                PhoneNumber = phoneNumber.Trim(),
                TermsAgreed = true,
                Status = TestRideStatus.Pending,
                CreatedDate = DateTime.UtcNow
            };

            _context.TestRides.Add(testRide);
            await _context.SaveChangesAsync();

            TempData["Success"] = "თქვენი მოთხოვნა წარმატებით გაიგზავნა! ჩვენი წარმომადგენელი მალე დაგიკავშირდებათ ამ ნომერზე.";
            return RedirectToAction("TestRideConfirmation", new { id = testRide.Id });
        }

        public async Task<IActionResult> TestRideConfirmation(int id)
        {
            var testRide = await _context.TestRides.FindAsync(id);
            if (testRide == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null || testRide.UserId != user.Id)
            {
                return Unauthorized();
            }

            return View(testRide);
        }

        public async Task<IActionResult> MyTestRides()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var testRides = _context.TestRides
                .Where(tr => tr.UserId == user.Id)
                .OrderByDescending(tr => tr.CreatedDate)
                .ToList();

            return View(testRides);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ConfirmTestRide(int id)
        {
            var testRide = await _context.TestRides.FindAsync(id);
            if (testRide == null)
            {
                return NotFound();
            }

            testRide.Status = TestRideStatus.Confirmed;
            _context.TestRides.Update(testRide);
            await _context.SaveChangesAsync();

            TempData["Success"] = "ტესტ-დრაივი დადასტურებულია.";
            return RedirectToAction("TestRides", "Admin");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CancelTestRide(int id)
        {
            var testRide = await _context.TestRides.FindAsync(id);
            if (testRide == null)
            {
                return NotFound();
            }

            testRide.Status = TestRideStatus.Cancelled;
            _context.TestRides.Update(testRide);
            await _context.SaveChangesAsync();

            TempData["Success"] = "ტესტ-დრაივი გაუქმებულია.";
            return RedirectToAction("TestRides", "Admin");
        }
    }
}
