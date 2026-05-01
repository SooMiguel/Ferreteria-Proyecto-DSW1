using System.ComponentModel.DataAnnotations;

namespace Proyecto_Ferreteria.Models
{
    public class Rol
    {
        [Key]
        public int IdRol { get; set; }
        public string Nombre { get; set; }
    }
}