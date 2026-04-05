using System;

namespace Distribuidora.Application.DTOs
{
    /// <summary>
    /// DTO para crear un nuevo Producto
    /// </summary>
    public class CreateProductDto
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int ProductTypeId { get; set; }
        public decimal? Price { get; set; }
    }

    /// <summary>
    /// DTO para actualizar un Producto
    /// </summary>
    public class UpdateProductDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int ProductTypeId { get; set; }
        public decimal? Price { get; set; }
    }

    /// <summary>
    /// DTO para respuesta de Producto
    /// </summary>
    public class ProductDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int ProductTypeId { get; set; }
        public string? ProductTypeName { get; set; }
        public decimal? Price { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
