using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_Ferreteria.Data;
using Proyecto_Ferreteria.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // <-- Agregado para seguridad

namespace Proyecto_Ferreteria.Controllers
{
    // <-- Candado para permitir solo Administradores
    [Authorize(Roles = "Administrador")]
    public class MarcasController : Controller
    {
        private readonly FerreteriaContext _context;

        public MarcasController(FerreteriaContext context)
        {
            _context = context;
        }

        // GET: Listado de Marcas
        public async Task<IActionResult> Index()
        {
            var listado = await _context.Marcas.ToListAsync();
            return View(listado);
        }

        // GET: Formulario de Registro (Create)
        public IActionResult Create()
        {
            return View();
        }

        // POST: Procesar el Registro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Marca objMarca)
        {
            if (ModelState.IsValid)
            {
                _context.Marcas.Add(objMarca);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(objMarca);
        }

        // GET: Formulario de Edición (Edit)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var marca = await ObtenerMarcaXID(id);
            if (marca == null) return NotFound();
            return View(marca);
        }

        // POST: Procesar la Edición
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Marca objMarca)
        {
            if (ModelState.IsValid)
            {
                _context.Marcas.Update(objMarca);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(objMarca);
        }

        // GET: Ver Detalles de la Marca
        public async Task<IActionResult> Details(int id)
        {
            var marca = await ObtenerMarcaXID(id);
            if (marca == null) return NotFound();
            return View(marca);
        }

        // GET: Formulario de Confirmación de Borrado (Delete)
        public async Task<IActionResult> Delete(int id)
        {
            var marca = await ObtenerMarcaXID(id);
            if (marca == null) return NotFound();
            return View(marca);
        }

        // POST: Procesar la Eliminación definitiva
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var marca = await _context.Marcas.FindAsync(id);
            if (marca != null)
            {
                _context.Marcas.Remove(marca);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // MÉTODO DE APOYO: Buscar Marca por ID
        public async Task<Marca> ObtenerMarcaXID(int id)
        {
            return await _context.Marcas.FindAsync(id);
        }
    }
}