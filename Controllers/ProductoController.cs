using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto_Ferreteria.Data;
using Proyecto_Ferreteria.Models;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization; // <-- 1. LIBRERÍA DE SEGURIDAD AGREGADA

namespace Proyecto_Ferreteria.Controllers
{
    // <-- 2. CANDADO QUE BLOQUEA EL ACCESO A CLIENTES O USUARIOS NO LOGUEADOS
    [Authorize(Roles = "Administrador")]
    public class ProductoController : Controller
    {
        private readonly FerreteriaContext _context;

        public ProductoController(FerreteriaContext context)
        {
            _context = context;
        }

        // GET: Lista de Productos activos
        public IActionResult Index()
        {
            var lista = _context.Productos.FromSqlRaw("EXEC sp_ListarProductos").ToList();

            // Pasamos las marcas y categorías para buscarlas en la vista
            ViewBag.ListaMarcas = _context.Marcas.ToList();
            ViewBag.ListaCategorias = _context.Categorias.ToList();

            return View(lista);
        }

        // GET: Mostrar formulario para agregar
        public IActionResult Agregar()
        {
            // Dropdowns (SelectLists) para la vista
            ViewBag.Marcas = new SelectList(_context.Marcas, "IdMarca", "Nombre");
            ViewBag.Categorias = new SelectList(_context.Categorias, "IdCategoria", "Nombre");

            return View();
        }

        // POST: Guardar el nuevo producto
        [HttpPost]
        public IActionResult Agregar(Producto obj)
        {
            if (ModelState.IsValid)
            {
                // Creamos los parámetros de forma explícita
                var parametros = new[]
                {
            new SqlParameter("@Nombre", obj.Nombre),
            new SqlParameter("@Descripcion", obj.Descripcion ?? (object)DBNull.Value),
            new SqlParameter("@IdMarca", obj.IdMarca),
            new SqlParameter("@IdCategoria", obj.IdCategoria),
            new SqlParameter("@Precio", obj.Precio),
            new SqlParameter("@Stock", obj.Stock),
            new SqlParameter("@ImagenUrl", obj.ImagenUrl ?? (object)DBNull.Value)
        };

                // Ejecutamos usando los nombres de los parámetros del SP
                _context.Database.ExecuteSqlRaw("EXEC sp_InsertarProducto @Nombre, @Descripcion, @IdMarca, @IdCategoria, @Precio, @Stock, @ImagenUrl", parametros);

                return RedirectToAction("Index");
            }

            ViewBag.Marcas = new SelectList(_context.Marcas, "IdMarca", "Nombre", obj.IdMarca);
            ViewBag.Categorias = new SelectList(_context.Categorias, "IdCategoria", "Nombre", obj.IdCategoria);
            return View(obj);
        }

        // GET: Mostrar formulario de edición
        public IActionResult Editar(int id)
        {
            // Buscamos el producto por su ID
            var producto = _context.Productos.Find(id);

            if (producto == null) return NotFound();

            // Llenamos los combos con el valor seleccionado actualmente
            ViewBag.Marcas = new SelectList(_context.Marcas, "IdMarca", "Nombre", producto.IdMarca);
            ViewBag.Categorias = new SelectList(_context.Categorias, "IdCategoria", "Nombre", producto.IdCategoria);

            return View(producto);
        }

        // POST: Procesar la actualización con el SP
        [HttpPost]
        public IActionResult Editar(Producto obj)
        {
            if (ModelState.IsValid)
            {
                var parametros = new[]
                {
            new SqlParameter("@IdProducto", obj.IdProducto),
            new SqlParameter("@Nombre", obj.Nombre),
            new SqlParameter("@Descripcion", obj.Descripcion ?? (object)DBNull.Value),
            new SqlParameter("@IdMarca", obj.IdMarca),
            new SqlParameter("@IdCategoria", obj.IdCategoria),
            new SqlParameter("@Precio", obj.Precio),
            new SqlParameter("@Stock", obj.Stock),
            new SqlParameter("@ImagenUrl", obj.ImagenUrl ?? (object)DBNull.Value)
        };

                // Ejecutamos el SP de actualización
                _context.Database.ExecuteSqlRaw("EXEC sp_ActualizarProducto @IdProducto, @Nombre, @Descripcion, @IdMarca, @IdCategoria, @Precio, @Stock, @ImagenUrl", parametros);

                return RedirectToAction("Index");
            }

            ViewBag.Marcas = new SelectList(_context.Marcas, "IdMarca", "Nombre", obj.IdMarca);
            ViewBag.Categorias = new SelectList(_context.Categorias, "IdCategoria", "Nombre", obj.IdCategoria);
            return View(obj);
        }

        // GET: Eliminar (Borrado Lógico)
        public IActionResult Eliminar(int id)
        {
            var producto = _context.Productos
                .Include(p => p.Marca)
                .Include(p => p.Categoria)
                .FirstOrDefault(p => p.IdProducto == id);

            if (producto == null) return NotFound();

            return View(producto);
        }

        // POST: Confirmar Eliminación
        [HttpPost, ActionName("Eliminar")]
        public IActionResult EliminarConfirmado(int id)
        {
            // Ejecutamos el SP de eliminación lógica
            _context.Database.ExecuteSqlRaw("EXEC sp_EliminarProducto @p0", id);

            return RedirectToAction("Index");
        }
    }
}