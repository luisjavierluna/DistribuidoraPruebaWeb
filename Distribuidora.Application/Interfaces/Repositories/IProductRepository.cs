using Distribuidora.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Distribuidora.Application.Interfaces.Repositories
{
    /// <summary>
    /// Interfaz para el repositorio de Productos
    /// Define los contratos para operaciones CRUD
    /// </summary>
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task<IEnumerable<Product>> GetByProductTypeAsync(int productTypeId);
        Task<int> AddAsync(Product product);
        Task<bool> UpdateAsync(Product product);
        Task<bool> DeleteAsync(int id);
    }
}
