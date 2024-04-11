using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Example.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Call the database initializer
using (var services = app.Services.CreateScope())
{
    var db = services.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    ApplicationDbInitializer.Initialize(db);
}

// Configure the HTTP request pipeline.
if (App.Environment.IsDevelopment()) // Database used during development
{
    // Register the database context as a service. Use SQLite for this
    services.AddDbContext<ExampleDbContext>(options =>
        options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
}
else // Database used in all other environments (production etc)
{
    // Register the database context as a service. Use PostgreSQL server for this
    services.AddDbContext<ExampleDbContext>(options =>
        options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
}

// Call the database initializer
using (var services = app.Services.CreateScope())
{
    var db = services.ServiceProvider.GetRequiredService<ExampleDbContext>();

    if (App.Environment.IsDevelopment()) // Database initialization
        ApplicationDbInitializer.Initialize(db);
    else
        await db.Database.MigrateAsync();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
