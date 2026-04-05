namespace WebAppMVC.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string? Clave { get; set; }
        public string? Nombre { get; set; }
        public int ProductTypeId { get; set; }
        public string? TipoProducto { get; set; }
        public bool EsActivo { get; set; }
        public decimal Precio { get; set; }
        public List<Proveedor> Proveedores { get; set; } = new List<Proveedor>();
    }
}
