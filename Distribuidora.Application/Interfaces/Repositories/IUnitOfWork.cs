using System;
using System.Threading.Tasks;

namespace Distribuidora.Application.Interfaces.Repositories
{
    /// <summary>
    /// Patrón Unit of Work
    /// Coordina múltiples repositorios y gestiona transacciones
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        ISupplierRepository Suppliers { get; }
        IProductTypeRepository ProductTypes { get; }
        IProductSupplierRepository ProductSuppliers { get; }

        Task<int> SaveChangesAsync();
    }
}
