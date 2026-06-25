using Microsoft.AspNetCore.Identity;

namespace vroom.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public override string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsAdmin { get; set; }

        public virtual List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public virtual List<Order> Orders { get; set; } = new List<Order>();
        public virtual List<TestRide> TestRides { get; set; } = new List<TestRide>();
    }
}

