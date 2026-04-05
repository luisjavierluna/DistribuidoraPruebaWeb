using Distribuidora.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Distribuidora.Application.Interfaces.Repositories
{
    /// <summary>
    /// Interfaz para el repositorio de Tipos de Producto
    /// Define los contratos para operaciones CRUD sobre catálogo
    /// </summary>
    public interface IProductTypeRepository
    {
        Task<IEnumerable<ProductType>> GetAllAsync();
        Task<ProductType?> GetByIdAsync(int id);
        Task<int> AddAsync(ProductType productType);
        Task<bool> UpdateAsync(ProductType productType);
        Task<bool> DeleteAsync(int id);
    }
}
