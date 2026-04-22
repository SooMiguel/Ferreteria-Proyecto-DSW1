using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation; //NUEVA LIBRERÍA

namespace Proyecto_Ferreteria.Models
{
    public class Producto
    {
        [Key]
        public int IdProducto { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = null!;

        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "Seleccione una marca")]
        public int IdMarca { get; set; }

        [ForeignKey("IdMarca")]
        [ValidateNever] // Le dice a C# que no exija el objeto completo desde el formulario
        public virtual Marca Marca { get; set; } = null!;

        [Required(ErrorMessage = "Seleccione una categoría")]
        public int IdCategoria { get; set; }

        [ForeignKey("IdCategoria")]
        [ValidateNever]
        public virtual Categoria Categoria { get; set; } = null!;

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "El stock es obligatorio")]
        public int Stock { get; set; }

        public string? ImagenUrl { get; set; }

        public bool Activo { get; set; } = true;
    }
}