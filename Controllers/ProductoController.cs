using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto_Ferreteria.Data;
using Proyecto_Ferreteria.Models;

namespace Proyecto_Ferreteria.Controllers
{
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
            // Include para traer datos de las tablas relacionadas
            var lista = _context.Productos
                .Include(p => p.Marca)
                .Include(p => p.Categoria)
                .Where(p => p.Activo == true) // Para mostrar los que no estan "eliminados"
                .ToList();

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
            // Se fuerza aca para que Activo sea true por defecto
            obj.Activo = true;

            if (ModelState.IsValid)
            {
                _context.Productos.Add(obj);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            // Para recargar las listas en caso de errror en la validacion
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
            var producto = _context.Productos.Find(id);
            if (producto != null)
            {
                // En vez de .Remove(), usemos el borrado lógico
                producto.Activo = false;
                _context.Productos.Update(producto);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}