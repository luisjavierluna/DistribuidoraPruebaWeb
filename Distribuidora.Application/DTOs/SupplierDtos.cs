using System;

namespace Distribuidora.Application.DTOs
{
    /// <summary>
    /// DTO para crear un nuevo Proveedor
    /// </summary>
    public class CreateSupplierDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    /// <summary>
    /// DTO para actualizar un Proveedor
    /// </summary>
    public class UpdateSupplierDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    /// <summary>
    /// DTO para respuesta de Proveedor
    /// </summary>
    public class SupplierDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
