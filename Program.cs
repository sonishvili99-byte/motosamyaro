using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using vroom.Data;
using vroom.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure UTF-8 encoding for proper Georgian text support
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

// Add Database Context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=(localdb)\\mssqllocaldb;Database=VroomDb;Trusted_Connection=true;";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add Identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Add session and cookie services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var app = builder.Build();

// Seed admin user, roles, and motorcycles at startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var context = services.GetRequiredService<ApplicationDbContext>();
        var config = services.GetRequiredService<IConfiguration>();

        // Seed motorcycles if they don't exist
        if (!context.Motorcycles.Any())
        {
            var motorcycles = new List<Motorcycle>
            {
                new Motorcycle 
                { 
                    Make = "Harley-Davidson", 
                    Model = "Street 750",
                    Year = 2019,
                    Category = "კრუიზერი",
                    Price = 7299m,
                    ImageUrl = "https://maintenanceschedule.com/wp-content/uploads/2024/02/Harley-Davidsion-Street-750-RHS-Red.jpg",
                    GalleryImages = new List<string>
                    {
                        "https://maintenanceschedule.com/wp-content/uploads/2024/02/Harley-Davidsion-Street-750-RHS-Red.jpg",
                        "https://mcn-images.bauersecure.com/wp-images/4372/1440x960/acivt519.jpg?mode=max&quality=90&scale=down",
                        "https://mcn-images.bauersecure.com/wp-images/4372/900x0/aciul392.jpg",
                        "https://mcn-images.bauersecure.com/wp-images/4372/900x0/harley_street_750_06.jpg"
                    },
                    Engine = "750cc V-Twin",
                    Horsepower = "52 HP / 38.8 kW",
                    Torque = "67 Nm",
                    FuelCapacity = "14 L",
                    TopSpeed = "193 km/h"
                },
                new Motorcycle 
                { 
                    Make = "Yamaha", 
                    Model = "YZF-R7", 
                    Year = 2025, 
                    Category = "სპორტი",
                    Price = 6799m, 
                    ImageUrl = "https://cdn.dealerspike.com/imglib/v1/800x600/imglib/trimsdb/23420811-10978891-138508541.png",
                    GalleryImages = new List<string>
                    {
                        "https://cdn.dealerspike.com/imglib/v1/800x600/imglib/trimsdb/23420811-10978891-138508541.png",
                        "https://cdn-tp3.mozu.com/27878-44719/cms/files/1f232144-f871-488e-8fb4-86c76f02295c?_mzcb=_1750240705528&width=1009&quality=70",
                        "https://cdn.dealerspike.com/imglib/v1/800x600/imglib/Assets/Inventory/9F/54/9F54DD8B-80D2-4157-AD3D-E03BEC9485CC.jpg",
                        "https://cdpcdn.dx1app.com/products/USA/YA/2025/MC/SPORT/YZF-R7/50/MATTE_RAVEN_BLACK/2000000014.jpg"
                    },
                    Engine = "689cc Parallel Twin",
                    Horsepower = "73.5 HP / 54.8 kW",
                    Torque = "67.3 Nm",
                    FuelCapacity = "15 L",
                    TopSpeed = "217 km/h"
                },
                new Motorcycle 
                { 
                    Make = "Yamaha", 
                    Model = "MT-03", 
                    Year = 2023, 
                    Category = "სტრიტი",
                    Price = 5899m,
                    ImageUrl = "https://www.zabikers.co.za/wp-content/uploads/2021/06/MT03-2.jpg",
                    GalleryImages = new List<string>
                    {
                        "https://www.zabikers.co.za/wp-content/uploads/2021/06/MT03-2.jpg",
                        "https://i.redd.it/u2o6vcq0thsb1.jpg",
                        "https://www.webbikeworld.com/wp-content/uploads/2023/03/2023-Yamaha-MT-03-9.jpg",
                        "https://carupdater.com/carlibrary/MRMotorcycles/large/999830903_1.jpg"
                    },
                    Engine = "321cc Parallel Twin",
                    Horsepower = "41.7 HP / 31.1 kW",
                    Torque = "41.2 Nm",
                    FuelCapacity = "14.8 L",
                    TopSpeed = "209 km/h"
                },
                new Motorcycle 
                { 
                    Make = "Triumph", 
                    Model = "Bonneville Bobber", 
                    Year = 2020, 
                    Category = "ბობერი",
                    Price = 9200m,
                    ImageUrl = "https://images5.1000ps.net/images_bikekat/2020/37-Triumph/9146-Bonneville_Bobber_Black/001-637118370088373491.jpg?format=webp&quality=80&trim.threshold=80&trim.percentpadding=1&scale=both&width=1168&height=664&bgcolor=rgba_39_42_44_0&mode=pad",
                    GalleryImages = new List<string>
                    {
                        "https://bringatrailer.com/wp-content/uploads/2024/02/2020_triumph_thruxton-tfc_img_7345-21409.jpeg?fit=940%2C626",
                        "https://www.gearpatrol.com/wp-content/uploads/sites/2/2024/12/triumph-factory-custom-bobber-macro-jpg.webp",
                        "https://upload.wikimedia.org/wikipedia/commons/0/05/2020-Triumph-Bobber-TFC-359.jpg",
                        "https://bringatrailer.com/wp-content/uploads/2024/02/2020_triumph_thruxton-tfc_img_7355-21454.jpeg?w=620&resize=620%2C413"
                    },
                    Engine = "1200cc Parallel Twin",
                    Horsepower = "80 HP / 59.7 kW",
                    Torque = "104.4 Nm",
                    FuelCapacity = "17 L",
                    TopSpeed = "220 km/h"
                },
                new Motorcycle 
                { 
                    Make = "Kawasaki", 
                    Model = "Ninja 400", 
                    Year = 2023, 
                    Category = "სპორტი",
                    Price = 4699m,
                    ImageUrl = "https://www.cycleworld.com/resizer/CTL-gFj6mOtAIRKGGI2Rm3dt3co=/1440x0/smart/cloudfront-us-east-1.images.arcpublishing.com/octane/SZXGFRI4URFRZBFCD4XSDZEIHA.jpg",
                    GalleryImages = new List<string>
                    {
                        "https://www.cycleworld.com/resizer/CTL-gFj6mOtAIRKGGI2Rm3dt3co=/1440x0/smart/cloudfront-us-east-1.images.arcpublishing.com/octane/SZXGFRI4URFRZBFCD4XSDZEIHA.jpg",
                        "https://live.staticflickr.com/65535/52789146495_2e8a66e38c_c.jpg",
                        "https://cdn.dealerspike.com/imglib/v1/800x600/imglib/Assets/Inventory/20/ED/20EDD737-C64F-4B4F-98F2-773C618E7FC7.jpg",
                        "https://d1y3j7h4632t1t.cloudfront.net/uploads/gallery/image/72224/large_20241116_130217.jpg"
                    },
                    Engine = "399cc Parallel Twin",
                    Horsepower = "44.8 HP / 33.4 kW",
                    Torque = "37 Nm",
                    FuelCapacity = "14 L",
                    TopSpeed = "206 km/h"
                },
                new Motorcycle 
                { 
                    Make = "Indian", 
                    Model = "Scout Bobber", 
                    Year = 2024, 
                    Category = "ბობერი",
                    Price = 8500m,
                    ImageUrl = "https://fasterwheeler.com/product_images/webp/indian-SCOUT-BOBBER-2025.webp",
                    GalleryImages = new List<string>
                    {
                        "https://fasterwheeler.com/product_images/webp/indian-SCOUT-BOBBER-2025.webp",
                        "https://s1.cdn.autoevolution.com/images/moto_gallery/indian-scout-bobber-twenty-2024-14689_1.jpg",
                        "https://www.motorcyclecruiser.com/resizer/HlnsXMy4gp1rUo_W320cT1yVG5M=/arc-photo-octane/arc3-prod/public/E3IGAU34MBBC3LIY6BEI5NVFKU.jpg",
                        "https://www.datocms-assets.com/119921/1714062546-2024-indian-scout-review-details-price-spec_07.jpg?auto=format&w=800"
                    },
                    Engine = "1133cc V-Twin",
                    Horsepower = "100 HP / 74.6 kW",
                    Torque = "97.6 Nm",
                    FuelCapacity = "13.2 L",
                    TopSpeed = "217 km/h"
                },
                new Motorcycle 
                { 
                    Make = "Ducati", 
                    Model = "Monster", 
                    Year = 2025, 
                    Category = "სტრიტი",
                    Price = 9999m,
                    ImageUrl = "https://i0.wp.com/www.asphaltandrubber.com/wp-content/uploads/2015/09/2016-Ducati-Monster-1200-R-studio-13-scaled.jpg?fit=2560%2C1917&ssl=1",
                    GalleryImages = new List<string>
                    {
                        "https://i0.wp.com/www.asphaltandrubber.com/wp-content/uploads/2015/09/2016-Ducati-Monster-1200-R-studio-13-scaled.jpg?fit=2560%2C1917&ssl=1",
                        "https://cdn.dealerspike.com/imglib/v1/800x600/imglib/Assets/Inventory/72/3A/723AC8BB-4D66-4245-8734-1A71A677B85C.jpg",
                        "https://cdpcdn.dx1app.com/products/USA/DU/2025/MC/SPORT/MONSTER_SP/50/DARK_STEALTH/2000000043.jpg",
                        "https://cdn.dealerspike.com/imglib/v1/800x600/imglib/Assets/Inventory/6B/04/6B045996-A1D8-431A-9E06-C79B28F9E3E5.jpg"
                    },
                    Engine = "1200cc L-Twin",
                    Horsepower = "147 HP / 109.7 kW",
                    Torque = "124.8 Nm",
                    FuelCapacity = "15 L",
                    TopSpeed = "233 km/h"
                }
            };
            context.Motorcycles.AddRange(motorcycles);
            context.SaveChanges();
        }

        var adminEmail = config["Admin:Email"] ?? "admin@vroom.local";
        var adminPassword = config["Admin:Password"] ?? "Admin123!";

        // create Admin role if it doesn't exist
        if (!roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
        {
            roleManager.CreateAsync(new IdentityRole("Admin")).GetAwaiter().GetResult();
        }

        // create admin user if missing
        var adminUser = userManager.FindByEmailAsync(adminEmail).GetAwaiter().GetResult();
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Admin",
                CreatedAt = DateTime.UtcNow,
                IsAdmin = true
            };
            var result = userManager.CreateAsync(adminUser, adminPassword).GetAwaiter().GetResult();
            if (result.Succeeded)
            {
                userManager.AddToRoleAsync(adminUser, "Admin").GetAwaiter().GetResult();
            }
        }
        else
        {
            // ensure user has Admin role
            if (!userManager.IsInRoleAsync(adminUser, "Admin").GetAwaiter().GetResult())
            {
                userManager.AddToRoleAsync(adminUser, "Admin").GetAwaiter().GetResult();
            }
        }
    }
    catch
    {
        // seeding should not crash the app on startup in development
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Set UTF-8 encoding BEFORE routing and view engine processing
app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        // Force UTF-8 charset on all HTML responses
        if (!context.Response.Headers.ContainsKey("Content-Type"))
        {
            context.Response.ContentType = "text/html; charset=utf-8";
        }
        else
        {
            var contentType = context.Response.Headers["Content-Type"].ToString();
            if (!contentType.Contains("charset"))
            {
                context.Response.Headers["Content-Type"] = contentType + "; charset=utf-8";
            }
        }
        
        context.Response.Headers["X-UA-Compatible"] = "IE=edge";
        context.Response.Headers["Content-Language"] = "ka";
        
        return System.Threading.Tasks.Task.CompletedTask;
    });
    
    await next();
});

app.UseStaticFiles();

app.UseRouting();



app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
name: "default",
pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

