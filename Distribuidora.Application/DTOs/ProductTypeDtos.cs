using System;

namespace Distribuidora.Application.DTOs
{
    /// <summary>
    /// DTO para crear un nuevo Tipo de Producto
    /// </summary>
    public class CreateProductTypeDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    /// <summary>
    /// DTO para actualizar un Tipo de Producto
    /// </summary>
    public class UpdateProductTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    /// <summary>
    /// DTO para respuesta de Tipo de Producto
    /// </summary>
    public class ProductTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
