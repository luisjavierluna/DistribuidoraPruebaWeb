using Distribuidora.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Distribuidora.Application.Interfaces.Repositories
{
    /// <summary>
    /// Interfaz para el repositorio de ProductoProveedor
    /// Define los contratos para gestionar la relación N:N
    /// </summary>
    public interface IProductSupplierRepository
    {
        Task<IEnumerable<ProductSupplier>> GetByProductIdAsync(int productId);
        Task<ProductSupplier?> GetByIdAsync(int id);
        Task<int> AddAsync(ProductSupplier productSupplier);
        Task<bool> UpdateAsync(ProductSupplier productSupplier);
        Task<bool> DeleteAsync(int id);
    }
}
