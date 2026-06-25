namespace vroom.Models
{
    public class Motorcycle
    {
        public int Id { get; set; }
        public required string Make { get; set; }
        public required string Model { get; set; }
        public int Year { get; set; }
        public required string Category { get; set; }
        public decimal Price { get; set; }
        public required string ImageUrl { get; set; }
        public List<string> GalleryImages { get; set; } = new List<string>();
        public required string Engine { get; set; }
        public required string Horsepower { get; set; }
        public required string Torque { get; set; }
        public required string FuelCapacity { get; set; }
        public required string TopSpeed { get; set; }
    }
}
