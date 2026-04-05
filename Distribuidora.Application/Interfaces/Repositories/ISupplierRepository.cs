using Distribuidora.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Distribuidora.Application.Interfaces.Repositories
{
    /// <summary>
    /// Interfaz para el repositorio de Proveedores
    /// Define los contratos para operaciones CRUD sobre catálogo
    /// </summary>
    public interface ISupplierRepository
    {
        Task<IEnumerable<Supplier>> GetAllAsync();
        Task<Supplier?> GetByIdAsync(int id);
        Task<int> AddAsync(Supplier supplier);
        Task<bool> UpdateAsync(Supplier supplier);
        Task<bool> DeleteAsync(int id);
    }
}
