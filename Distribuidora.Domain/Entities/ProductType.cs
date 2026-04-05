using System.Collections.Generic;

namespace Distribuidora.Domain.Entities
{
    /// <summary>
    /// Catálogo de tipos de producto
    /// </summary>
    public class ProductType : BaseEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }

        // Relación
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
