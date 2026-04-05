using System;

namespace Distribuidora.Application.DTOs
{
    /// <summary>
    /// DTO para crear una nueva relación Producto-Proveedor
    /// </summary>
    public class CreateProductSupplierDto
    {
        public int ProductId { get; set; }
        public int SupplierId { get; set; }
        public string? SupplierProductCode { get; set; }
        public decimal Cost { get; set; }
    }

    /// <summary>
    /// DTO para actualizar una relación Producto-Proveedor
    /// </summary>
    public class UpdateProductSupplierDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int SupplierId { get; set; }
        public string? SupplierProductCode { get; set; }
        public decimal Cost { get; set; }
    }

    /// <summary>
    /// DTO para respuesta de relación Producto-Proveedor
    /// </summary>
    public class ProductSupplierDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public string? SupplierProductCode { get; set; }
        public decimal Cost { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
