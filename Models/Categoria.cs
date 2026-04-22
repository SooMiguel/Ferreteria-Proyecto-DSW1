using System.ComponentModel.DataAnnotations;

namespace Proyecto_Ferreteria.Models
{
    public class Categoria
    {
        [Key]
        public int IdCategoria { get; set; }

        [Required]
        public string Nombre { get; set; }
    }
}