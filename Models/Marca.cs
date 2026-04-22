using System.ComponentModel.DataAnnotations;

namespace Proyecto_Ferreteria.Models
{
    public class Marca
    {
        [Key]
        public int IdMarca { get; set; }

        [Required]
        public string Nombre { get; set; }
    }
}