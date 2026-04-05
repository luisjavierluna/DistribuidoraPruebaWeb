using System.Collections.Generic;

namespace Distribuidora.Domain.Entities
{
    /// <summary>
    /// Entidad de Producto
    /// Depende de ProductType
    /// Puede existir sin proveedores
    /// </summary>
    public class Product : BaseEntity
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public int ProductTypeId { get; set; }
        public decimal? Price { get; set; }

        // Relaciones
        public ProductType? ProductType { get; set; }
        public ICollection<ProductSupplier> ProductSuppliers { get; set; } = new List<ProductSupplier>();
    }
}
