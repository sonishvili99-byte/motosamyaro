namespace vroom.Models
{
    public class TestRide
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public bool TermsAgreed { get; set; }
        public TestRideStatus Status { get; set; } = TestRideStatus.Pending;
        public string? Notes { get; set; }
    }

    public enum TestRideStatus
    {
        Pending,
        Confirmed,
        Cancelled,
        Completed
    }
}
