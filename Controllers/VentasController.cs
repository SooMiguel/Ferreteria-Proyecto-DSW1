using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Proyecto_Ferreteria.Data;
using Proyecto_Ferreteria.Models;
using Microsoft.AspNetCore.Authorization; // <-- Agregado para seguridad

namespace Proyecto_Ferreteria.Controllers
{
    // <-- Candado para permitir solo Administradores
    [Authorize(Roles = "Administrador")]
    public class VentasController : Controller
    {
        private readonly FerreteriaContext _context;

        public VentasController(FerreteriaContext context)
        {
            _context = context;
        }

        public IActionResult Index(DateTime? inicio, DateTime? fin)
        {
            // Si el usuario no selecciona fechas, mostramos las ventas del último mes por defecto
            DateTime fechaInicio = inicio ?? DateTime.Now.AddMonths(-1);
            DateTime fechaFin = fin ?? DateTime.Now;

            //Parámetros para SQL Server
            var parametros = new[]
            {
                new SqlParameter("@FechaInicio", fechaInicio),
                new SqlParameter("@FechaFin", fechaFin)
            };
            // Ejecutamos el Procedimiento Almacenado
            var lista = _context.VentasViewModel
                .FromSqlRaw("EXEC sp_ListarVentasPorFecha @FechaInicio, @FechaFin", parametros)
                .ToList();

            return View(lista);
        }
    }
}