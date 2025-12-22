using Microsoft.AspNetCore.Mvc;
using vroom.Models;

namespace vroom.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var motorcycles = GetMotorcycles();
            return View(motorcycles);
        }

        public IActionResult Category(string name)
        {
            var motorcycles = GetMotorcycles()
                .Where(m => m.Category == name) 
                .ToList();
            ViewData["Category"] = name;
            return View("Index", motorcycles);
        }

        public IActionResult About()
        {
            return View();
        }

        private List<Motorcycle> GetMotorcycles()
        {
            return new List<Motorcycle>
            {
                new Motorcycle 
                { 
                    Id = 1, 
                    Make = "Harley-Davidson", 
                    Model = "Street 750",
                    Year = 2019,
                    Category = "კრუიზერი",
                    Price = 7299m,
                    ImageUrl = "https://maintenanceschedule.com/wp-content/uploads/2024/02/Harley-Davidsion-Street-750-RHS-Red.jpg",
                    Engine = "750cc V-Twin",
                    Horsepower = "52 HP / 38.8 kW",
                    Torque = "67 Nm",
                    FuelCapacity = "14 L",
                    TopSpeed = "193 km/h"
                },
                new Motorcycle 
                { 
                    Id = 2, 
                    Make = "Yamaha", 
                    Model = "YZF-R7", 
                    Year = 2025, 
                    Category = "სპორტი",
                    Price = 6799m, 
                    ImageUrl = "https://cdn.dealerspike.com/imglib/v1/800x600/imglib/trimsdb/23420811-10978891-138508541.png",
                    Engine = "689cc Parallel Twin",
                    Horsepower = "73.5 HP / 54.8 kW",
                    Torque = "67.3 Nm",
                    FuelCapacity = "15 L",
                    TopSpeed = "217 km/h"
                },
                new Motorcycle 
                { 
                    Id = 3, 
                    Make = "Yamaha", 
                    Model = "MT-03", 
                    Year = 2023, 
                    Category = "სტრიტი",
                    Price = 5899m,
                    ImageUrl = "https://www.zabikers.co.za/wp-content/uploads/2021/06/MT03-2.jpg",
                    Engine = "321cc Parallel Twin",
                    Horsepower = "41.7 HP / 31.1 kW",
                    Torque = "41.2 Nm",
                    FuelCapacity = "14.8 L",
                    TopSpeed = "209 km/h"
                },
                new Motorcycle 
                { 
                    Id = 4, 
                    Make = "Triumph", 
                    Model = "Bonneville Bobber", 
                    Year = 2020, 
                    Category = "ბობერი",
                    Price = 9200m,
                    ImageUrl = "https://images5.1000ps.net/images_bikekat/2020/37-Triumph/9146-Bonneville_Bobber_Black/001-637118370088373491.jpg?format=webp&quality=80&trim.threshold=80&trim.percentpadding=1&scale=both&width=1168&height=664&bgcolor=rgba_39_42_44_0&mode=pad",
                    Engine = "1200cc Parallel Twin",
                    Horsepower = "80 HP / 59.7 kW",
                    Torque = "104.4 Nm",
                    FuelCapacity = "17 L",
                    TopSpeed = "220 km/h"
                },
                new Motorcycle 
                { 
                    Id = 5, 
                    Make = "Kawasaki", 
                    Model = "Ninja 400", 
                    Year = 2023, 
                    Category = "სპორტი",
                    Price = 4699m,
                    ImageUrl = "https://www.cycleworld.com/resizer/CTL-gFj6mOtAIRKGGI2Rm3dt3co=/1440x0/smart/cloudfront-us-east-1.images.arcpublishing.com/octane/SZXGFRI4URFRZBFCD4XSDZEIHA.jpg",
                    Engine = "399cc Parallel Twin",
                    Horsepower = "44.8 HP / 33.4 kW",
                    Torque = "37 Nm",
                    FuelCapacity = "14 L",
                    TopSpeed = "206 km/h"
                },
                new Motorcycle 
                { 
                    Id = 6, 
                    Make = "Indian", 
                    Model = "Scout Bobber", 
                    Year = 2024, 
                    Category = "ბობერი",
                    Price = 8500m,
                    ImageUrl = "https://fasterwheeler.com/product_images/webp/indian-SCOUT-BOBBER-2025.webp",
                    Engine = "1133cc V-Twin",
                    Horsepower = "100 HP / 74.6 kW",
                    Torque = "97.6 Nm",
                    FuelCapacity = "13.2 L",
                    TopSpeed = "217 km/h"
                },
                new Motorcycle 
                { 
                    Id = 7, 
                    Make = "Ducati", 
                    Model = "Monster", 
                    Year = 2025, 
                    Category = "სტრიტი",
                    Price = 9999m,
                    ImageUrl = "https://i0.wp.com/www.asphaltandrubber.com/wp-content/uploads/2015/09/2016-Ducati-Monster-1200-R-studio-13-scaled.jpg?fit=2560%2C1917&ssl=1",
                    Engine = "1200cc L-Twin",
                    Horsepower = "147 HP / 109.7 kW",
                    Torque = "124.8 Nm",
                    FuelCapacity = "15 L",
                    TopSpeed = "233 km/h"
                }
            };
        }
    }
}
