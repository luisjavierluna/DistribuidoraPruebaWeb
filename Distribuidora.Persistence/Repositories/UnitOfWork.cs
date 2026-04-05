using System;
using System.Threading.Tasks;
using Distribuidora.Application.Interfaces.Repositories;

namespace Distribuidora.Persistence.Repositories
{
    /// <summary>
    /// Implementación del patrón Unit of Work
    /// Coordina múltiples repositorios y gestiona transacciones
    /// </summary>
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly string _connectionString;
        private IProductRepository? _productRepository;
        private ISupplierRepository? _supplierRepository;
        private IProductTypeRepository? _productTypeRepository;
        private IProductSupplierRepository? _productSupplierRepository;

        public UnitOfWork(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        /// <summary>
        /// Repositorio de Productos
        /// </summary>
        public IProductRepository Products
        {
            get { return _productRepository ??= new ProductRepository(_connectionString); }
        }

        /// <summary>
        /// Repositorio de Proveedores
        /// </summary>
        public ISupplierRepository Suppliers
        {
            get { return _supplierRepository ??= new SupplierRepository(_connectionString); }
        }

        /// <summary>
        /// Repositorio de Tipos de Producto
        /// </summary>
        public IProductTypeRepository ProductTypes
        {
            get { return _productTypeRepository ??= new ProductTypeRepository(_connectionString); }
        }

        /// <summary>
        /// Repositorio de Relaciones Producto-Proveedor
        /// </summary>
        public IProductSupplierRepository ProductSuppliers
        {
            get { return _productSupplierRepository ??= new ProductSupplierRepository(_connectionString); }
        }

        /// <summary>
        /// En ADO.NET puro, no tiene transacciones explícitas a nivel de Unit of Work
        /// Las transacciones se manejan a nivel de stored procedure
        /// Retorna 1 si fue exitoso
        /// </summary>
        public async Task<int> SaveChangesAsync()
        {
            // En ADO.NET, SaveChanges no hace nada pues cada operación se ejecuta inmediatamente
            // Las transacciones se manejan dentro de cada stored procedure
            await Task.CompletedTask;
            return 1;
        }

        /// <summary>
        /// Libera recursos
        /// </summary>
        public void Dispose()
        {
            // ADO.NET no requiere disposición especial para repositorios
            // Las conexiones se cierran automáticamente en los using statements
            GC.SuppressFinalize(this);
        }
    }
}
