namespace WebAppMVC.Models
{
    public class Proveedor
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? ClaveProducto { get; set; }
        public decimal Costo { get; set; }
    }
}
