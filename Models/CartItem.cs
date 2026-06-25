namespace vroom.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int MotorcycleId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        public virtual Motorcycle? Motorcycle { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}

