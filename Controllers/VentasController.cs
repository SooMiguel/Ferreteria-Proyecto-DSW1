using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Proyecto_Ferreteria.Data;
using Proyecto_Ferreteria.Models;

namespace Proyecto_Ferreteria.Controllers
{
    public class VentasController : Controller
    {
        private readonly FerreteriaContext _context;

        public VentasController(FerreteriaContext context)
        {
            _context = context;
        }

        public IActionResult Index(DateTime? inicio, DateTime? fin)
        {
            DateTime fechaInicio = inicio ?? DateTime.Now.AddMonths(-1);
            DateTime fechaFin = fin ?? DateTime.Now;

            var parametros = new[]
            {
                new SqlParameter("@FechaInicio", fechaInicio),
                new SqlParameter("@FechaFin", fechaFin)
            };

            var lista = _context.VentasViewModel
                .FromSqlRaw("EXEC sp_ListarVentasPorFecha @FechaInicio, @FechaFin", parametros)
                .ToList();

            return View(lista);
        }
    }
}