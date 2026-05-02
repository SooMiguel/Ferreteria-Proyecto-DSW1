using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Proyecto_Ferreteria.Data;
using Microsoft.EntityFrameworkCore;
using Proyecto_Ferreteria.Models;
using System.Threading.Tasks;

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
            // Verificación: Si ya está logueado, evaluamos a dónde mandarlo
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                // Si es administrador, al panel de control (Home)
                if (User.IsInRole("Administrador"))
                {
                    return RedirectToAction("Index", "Home");
                }
                // Si es cliente, directo a comprar (Tienda)
                return RedirectToAction("Index", "Tienda");
            }
            return View();
        }

        // POST: Procesa el formulario
        [HttpPost]
        public async Task<IActionResult> Ingresar(string Correo, string Clave)
        {
            // 1. Buscamos al usuario en la BD (Incluimos el Rol)
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Correo == Correo && u.Clave == Clave);

            if (usuario != null)
            {
                // 2. Guardamos las credenciales y su Rol
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.Nombres),
                    new Claim("Correo", usuario.Correo),
                    new Claim(ClaimTypes.Role, usuario.Rol.Nombre),
                    new Claim("IdUsuario", usuario.IdUsuario.ToString())
                };

                // 3. Generamos la "Cookie" de sesión
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                // 4. REDIRECCIÓN INTELIGENTE SEGÚN EL ROL
                if (usuario.Rol.Nombre == "Administrador")
                {
                    // El jefe va a su panel de control
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Los clientes van a comprar
                    return RedirectToAction("Index", "Tienda");
                }
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