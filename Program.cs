using Microsoft.AspNetCore.Authentication.Cookies; // <-- LIBRERÍA AGREGADA PARA SEGURIDAD
using Microsoft.EntityFrameworkCore;
using Proyecto_Ferreteria.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// --- INICIO: CONFIGURACIÓN DEL ROL 2 (Seguridad y Sesiones) ---
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Index"; // Si no hay sesión, los redirige al login
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // La sesión expira en 30 min
        options.AccessDeniedPath = "/Home/Index"; // Si intentan entrar a un área prohibida
    });
// --- FIN: CONFIGURACIÓN DEL ROL 2 ---

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

// --- INICIO: ACTIVACIÓN EN EL PIPELINE (EL ORDEN ES VITAL) ---
app.UseAuthentication(); // <-- 1. Primero verifica QUIÉN eres (NUEVO)
app.UseAuthorization();  // <-- 2. Luego verifica QUÉ puedes hacer (El que ya tenías)
// --- FIN ---

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");
app.Run();