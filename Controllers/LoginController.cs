using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Proyecto_Ferreteria.Data;
using Microsoft.EntityFrameworkCore;
using Proyecto_Ferreteria.Models; // <-- NUEVO: Para que reconozca a los Usuarios y Roles
using System.Threading.Tasks; // <-- NUEVO: Para que funcionen los métodos async

namespace Proyecto_Ferreteria.Controllers
{
    public class LoginController : Controller
    {
        private readonly FerreteriaContext _context;

        public LoginController(FerreteriaContext context)
        {
            _context = context;
        }

        // GET: Muestra la pantalla de Login
        public IActionResult Index()
        {
            // Verificación segura: Si ya está logueado, lo mandamos al Home
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Procesa el formulario
        [HttpPost]
        public async Task<IActionResult> Ingresar(string Correo, string Clave)
        {
            // 1. Buscamos al usuario en la BD (Incluimos el Rol para saber si es Admin o Cliente)
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Correo == Correo && u.Clave == Clave);

            if (usuario != null)
            {
                // 2. ¡Las Credenciales! (Claims). Aquí guardamos su nombre, su rol y su ID.
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.Nombres),
                    new Claim("Correo", usuario.Correo),
                    new Claim(ClaimTypes.Role, usuario.Rol.Nombre),
                    new Claim("IdUsuario", usuario.IdUsuario.ToString()) // Vital para el carrito
                };

                // 3. Generamos la "Cookie" de sesión
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                // 4. Lo mandamos al sistema
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Si se equivoca de clave
                ViewBag.Error = "Correo o contraseña incorrectos.";
                return View("Index");
            }
        }

        // GET: Para cerrar la sesión
        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }
    }
}