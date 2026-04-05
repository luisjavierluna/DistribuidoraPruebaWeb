using System;

namespace Distribuidora.Domain
{
    /// <summary>
    /// Clase base para todas las entidades del dominio.
    /// Proporciona propiedades comunes: Id, Active, CreatedAt, UpdatedAt
    /// </summary>
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public bool Active { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
