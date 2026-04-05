using System.Collections.Generic;

namespace Distribuidora.Domain.Entities
{
    /// <summary>
    /// Catálogo de proveedores
    /// </summary>
    public class Supplier : BaseEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }

        // Relación
        public ICollection<ProductSupplier> ProductSuppliers { get; set; } = new List<ProductSupplier>();
    }
}
