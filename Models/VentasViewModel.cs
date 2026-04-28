namespace Proyecto_Ferreteria.Models
{
    public class VentasViewModel
    {
        public int IdVenta { get; set; }
        public string Cliente { get; set; }
        public DateTime FechaVenta { get; set; }
        public decimal ImporteTotal { get; set; }
        public string Estado { get; set; }
    }
}
