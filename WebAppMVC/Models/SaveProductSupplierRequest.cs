namespace WebAppMVC.Models
{
    /// <summary>
    /// DTO para recibir datos del formulario de proveedor en AJAX
    /// </summary>
    public class SaveProductSupplierRequest
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int SupplierId { get; set; }
        public string? SupplierProductCode { get; set; }
        public decimal Cost { get; set; }
    }
}
