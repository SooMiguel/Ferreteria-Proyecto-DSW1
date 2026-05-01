using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_Ferreteria.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization; // <-- NUEVO: Necesario para poder usar [Authorize]

namespace Proyecto_Ferreteria.Controllers
{
    public class TiendaController : Controller
    {
        private readonly FerreteriaContext _context;

        public TiendaController(FerreteriaContext context)
        {
            _context = context;
        }

        // GET: Vista de la Tienda
        public IActionResult Index()
        {
            var productosParaVender = _context.Productos
                .Include(p => p.Marca)
                .Include(p => p.Categoria)
                .Where(p => p.Activo == true && p.Stock > 0)
                .ToList();

            return View(productosParaVender);
        }

        // POST: Recibe el JSON por AJAX y guarda en SQL
        [Authorize] // <--- NUEVO: Candado de seguridad (Solo usuarios logueados)
        [HttpPost]
        public IActionResult ProcesarPago([FromBody] List<CarritoItem> carrito)
        {
            if (carrito == null || carrito.Count == 0)
                return Json(new { exito = false, mensaje = "El carrito está vacío." });

            // NUEVO: Leemos el ID directamente de la sesión (Cookie) del usuario conectado
            int idClienteActual = int.Parse(User.FindFirst("IdUsuario").Value);

            decimal totalPago = carrito.Sum(x => x.Precio * x.Cantidad);

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var idVentaParam = new SqlParameter
                {
                    ParameterName = "@IdVentaGenerado",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Output
                };

                _context.Database.ExecuteSqlRaw(
                    "INSERT INTO Ventas (IdUsuario, FechaVenta, ImporteTotal, Estado) " +
                    "VALUES (@p0, GETDATE(), @p1, 'Pagado'); " +
                    "SET @IdVentaGenerado = SCOPE_IDENTITY();", // Captura el ID de la venta
                    new SqlParameter("@p0", idClienteActual),
                    new SqlParameter("@p1", totalPago),
                    idVentaParam);

                int nuevaVentaId = (int)idVentaParam.Value;

                foreach (var item in carrito)
                {
                    decimal importeItem = item.Precio * item.Cantidad;

                    _context.Database.ExecuteSqlRaw(
                        "INSERT INTO DetalleVentas (IdVenta, IdProducto, Cantidad, PrecioUnitario, Importe) " +
                        "VALUES (@p0, @p1, @p2, @p3, @p4)",
                        new SqlParameter("@p0", nuevaVentaId),
                        new SqlParameter("@p1", item.Id),
                        new SqlParameter("@p2", item.Cantidad),
                        new SqlParameter("@p3", item.Precio),
                        new SqlParameter("@p4", importeItem));

                    _context.Database.ExecuteSqlRaw(
                        "UPDATE Productos SET Stock = Stock - @p0 WHERE IdProducto = @p1",
                        new SqlParameter("@p0", item.Cantidad),
                        new SqlParameter("@p1", item.Id));
                }

                // 3. Si todo salió perfecto, CONFIRMAMOS LA TRANSACCIÓN
                transaction.Commit();

                return Json(new { exito = true, mensaje = "¡Tu compra ha sido procesada con éxito!" });
            }
            catch (Exception ex)
            {
                // Si explota cualquier cosa, DESHACEMOS LA TRANSACCIÓN
                transaction.Rollback();
                return Json(new { exito = false, mensaje = "Ocurrió un error en el servidor: " + ex.Message });
            }
        }
    }

    public class CarritoItem
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
    }
}