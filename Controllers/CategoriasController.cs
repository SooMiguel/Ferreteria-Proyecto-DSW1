using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_Ferreteria.Data;
using Proyecto_Ferreteria.Models;
using System.Threading.Tasks;

namespace Proyecto_Ferreteria.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly FerreteriaContext _context;

        public CategoriasController(FerreteriaContext context)
        {
            _context = context;
        }

        // GET: Listado de Categorías
        public async Task<IActionResult> Index()
        {
            var listado = await _context.Categorias.ToListAsync();
            return View(listado);
        }

        // GET: Formulario de Registro
        public IActionResult Create()
        {
            return View();
        }

        // POST: Procesar el Registro
        [HttpPost]
        public async Task<IActionResult> Create(Categoria objCategoria)
        {
            if (ModelState.IsValid)
            {
                _context.Categorias.Add(objCategoria);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(objCategoria);
        }

        // GET: Formulario de Edición
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var categoria = await ObtenerCategoriaXID(id);
            if (categoria == null) return NotFound();
            return View(categoria);
        }

        // POST: Procesar la Edición
        [HttpPost]
        public async Task<IActionResult> Edit(Categoria objCategoria)
        {
            if (ModelState.IsValid)
            {
                _context.Categorias.Update(objCategoria);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(objCategoria);
        }

        // GET: Ver Detalles de Categoría
        public async Task<IActionResult> Details(int id)
        {
            var categoria = await ObtenerCategoriaXID(id);
            if (categoria == null) return NotFound();
            return View(categoria);
        }

        // GET: Formulario de Confirmación de Eliminación
        public async Task<IActionResult> Delete(int id)
        {
            var categoria = await ObtenerCategoriaXID(id);
            if (categoria == null) return NotFound();
            return View(categoria);
        }

        // POST: Procesar la Eliminación Definitiva
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria != null)
            {
                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // MÉTODO DE APOYO: Buscar por ID
        public async Task<Categoria> ObtenerCategoriaXID(int id)
        {
            return await _context.Categorias.FindAsync(id);
        }
    }
}