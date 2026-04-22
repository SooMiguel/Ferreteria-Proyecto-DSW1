using Microsoft.EntityFrameworkCore;
using Proyecto_Ferreteria.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configuracion de la BD usando la cadena del appsettings.json
builder.Services.AddDbContext<FerreteriaContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("cadena")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();