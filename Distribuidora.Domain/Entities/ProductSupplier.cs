using System.Collections.Generic;

namespace Distribuidora.Domain.Entities
{
    /// <summary>
    /// Tabla puente para relación N:N entre Productos y Proveedores
    /// Depende tanto de Producto como de Proveedor
    /// </summary>
    public class ProductSupplier : BaseEntity
    {
        public int ProductId { get; set; }
        public int SupplierId { get; set; }
        public string? SupplierProductCode { get; set; }
        public decimal Cost { get; set; }

        // Relaciones
        public Product? Product { get; set; }
        public Supplier? Supplier { get; set; }
    }
}
