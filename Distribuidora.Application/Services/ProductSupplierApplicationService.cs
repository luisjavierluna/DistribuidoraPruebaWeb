using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Distribuidora.Application.DTOs;
using Distribuidora.Application.Exceptions;
using Distribuidora.Application.Interfaces.Repositories;
using Distribuidora.Domain.Entities;
using AppException = Distribuidora.Application.Exceptions.ApplicationException;

namespace Distribuidora.Application.Services
{
    /// <summary>
    /// Servicio de aplicación para ProductSupplier
    /// Orquesta lógica de negocio para la relación N:N Producto-Proveedor
    /// </summary>
    public class ProductSupplierApplicationService : IDisposable
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductSupplierApplicationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Obtiene todos los proveedores de un producto
        /// </summary>
        public async Task<IEnumerable<ProductSupplierDto>> GetSuppliersByProductIdAsync(int productId)
        {
            if (productId <= 0)
                throw new InvalidProductException("El ProductId debe ser mayor a 0");

            try
            {
                // Verificar que el producto existe
                var product = await _unitOfWork.Products.GetByIdAsync(productId);
                if (product == null)
                    throw new InvalidProductException($"Producto con Id {productId} no encontrado");

                var productSuppliers = await _unitOfWork.ProductSuppliers.GetByProductIdAsync(productId);
                
                // Enriquecer con nombres de proveedores
                var suppliers = await _unitOfWork.Suppliers.GetAllAsync();
                var dtos = new List<ProductSupplierDto>();

                foreach (var ps in productSuppliers)
                {
                    var supplier = suppliers.FirstOrDefault(s => s.Id == ps.SupplierId);
                    dtos.Add(new ProductSupplierDto
                    {
                        Id = ps.Id,
                        ProductId = ps.ProductId,
                        SupplierId = ps.SupplierId,
                        SupplierName = supplier?.Name,
                        SupplierProductCode = ps.SupplierProductCode,
                        Cost = ps.Cost,
                        Active = ps.Active,
                        CreatedAt = ps.CreatedAt,
                        UpdatedAt = ps.UpdatedAt
                    });
                }

                return dtos;
            }
            catch (InvalidProductException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AppException($"Error al obtener proveedores del producto: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene un proveedor específico de un producto
        /// </summary>
        public async Task<ProductSupplierDto?> GetProductSupplierByIdAsync(int id)
        {
            if (id <= 0)
                throw new InvalidProductException("El Id debe ser mayor a 0");

            try
            {
                var productSupplier = await _unitOfWork.ProductSuppliers.GetByIdAsync(id);
                if (productSupplier == null)
                    return null;

                var supplier = await _unitOfWork.Suppliers.GetByIdAsync(productSupplier.SupplierId);

                return new ProductSupplierDto
                {
                    Id = productSupplier.Id,
                    ProductId = productSupplier.ProductId,
                    SupplierId = productSupplier.SupplierId,
                    SupplierName = supplier?.Name,
                    SupplierProductCode = productSupplier.SupplierProductCode,
                    Cost = productSupplier.Cost,
                    Active = productSupplier.Active,
                    CreatedAt = productSupplier.CreatedAt,
                    UpdatedAt = productSupplier.UpdatedAt
                };
            }
            catch (Exception ex)
            {
                throw new AppException($"Error al obtener relación producto-proveedor: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Crea una nueva relación Producto-Proveedor
        /// </summary>
        public async Task<ProductSupplierDto> CreateProductSupplierAsync(CreateProductSupplierDto createDto)
        {
            if (createDto == null)
                throw new ArgumentNullException(nameof(createDto));

            if (createDto.ProductId <= 0)
                throw new InvalidProductException("El ProductId debe ser mayor a 0");

            if (createDto.SupplierId <= 0)
                throw new InvalidProductException("El SupplierId debe ser mayor a 0");

            if (createDto.Cost < 0)
                throw new InvalidProductException("El costo no puede ser negativo");

            try
            {
                // Verificar que el producto y proveedor existen
                var product = await _unitOfWork.Products.GetByIdAsync(createDto.ProductId);
                if (product == null)
                    throw new InvalidProductException($"Producto con Id {createDto.ProductId} no encontrado");

                var supplier = await _unitOfWork.Suppliers.GetByIdAsync(createDto.SupplierId);
                if (supplier == null)
                    throw new InvalidProductException($"Proveedor con Id {createDto.SupplierId} no encontrado");

                var productSupplier = new ProductSupplier
                {
                    ProductId = createDto.ProductId,
                    SupplierId = createDto.SupplierId,
                    SupplierProductCode = createDto.SupplierProductCode,
                    Cost = createDto.Cost,
                    Active = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var newId = await _unitOfWork.ProductSuppliers.AddAsync(productSupplier);
                productSupplier.Id = newId;

                await _unitOfWork.SaveChangesAsync();

                return new ProductSupplierDto
                {
                    Id = productSupplier.Id,
                    ProductId = productSupplier.ProductId,
                    SupplierId = productSupplier.SupplierId,
                    SupplierName = supplier.Name,
                    SupplierProductCode = productSupplier.SupplierProductCode,
                    Cost = productSupplier.Cost,
                    Active = productSupplier.Active,
                    CreatedAt = productSupplier.CreatedAt,
                    UpdatedAt = productSupplier.UpdatedAt
                };
            }
            catch (InvalidProductException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AppException($"Error al crear relación producto-proveedor: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Actualiza una relación Producto-Proveedor existente
        /// </summary>
        public async Task<ProductSupplierDto> UpdateProductSupplierAsync(UpdateProductSupplierDto updateDto)
        {
            if (updateDto == null)
                throw new ArgumentNullException(nameof(updateDto));

            if (updateDto.Id <= 0)
                throw new InvalidProductException("El Id debe ser mayor a 0");

            if (updateDto.Cost < 0)
                throw new InvalidProductException("El costo no puede ser negativo");

            try
            {
                // Verificar que la relación existe
                var productSupplier = await _unitOfWork.ProductSuppliers.GetByIdAsync(updateDto.Id);
                if (productSupplier == null)
                    throw new InvalidProductException("Relación producto-proveedor no encontrada");

                // Verificar que el proveedor existe
                var supplier = await _unitOfWork.Suppliers.GetByIdAsync(updateDto.SupplierId);
                if (supplier == null)
                    throw new InvalidProductException($"Proveedor con Id {updateDto.SupplierId} no encontrado");

                productSupplier.SupplierId = updateDto.SupplierId;
                productSupplier.SupplierProductCode = updateDto.SupplierProductCode;
                productSupplier.Cost = updateDto.Cost;
                productSupplier.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.ProductSuppliers.UpdateAsync(productSupplier);
                await _unitOfWork.SaveChangesAsync();

                return new ProductSupplierDto
                {
                    Id = productSupplier.Id,
                    ProductId = productSupplier.ProductId,
                    SupplierId = productSupplier.SupplierId,
                    SupplierName = supplier.Name,
                    SupplierProductCode = productSupplier.SupplierProductCode,
                    Cost = productSupplier.Cost,
                    Active = productSupplier.Active,
                    CreatedAt = productSupplier.CreatedAt,
                    UpdatedAt = productSupplier.UpdatedAt
                };
            }
            catch (InvalidProductException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AppException($"Error al actualizar relación producto-proveedor: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Elimina una relación Producto-Proveedor
        /// </summary>
        public async Task<bool> DeleteProductSupplierAsync(int id)
        {
            if (id <= 0)
                throw new InvalidProductException("El Id debe ser mayor a 0");

            try
            {
                // Verificar que la relación existe
                var productSupplier = await _unitOfWork.ProductSuppliers.GetByIdAsync(id);
                if (productSupplier == null)
                    throw new InvalidProductException("Relación producto-proveedor no encontrada");

                var result = await _unitOfWork.ProductSuppliers.DeleteAsync(id);
                if (result)
                {
                    await _unitOfWork.SaveChangesAsync();
                }

                return result;
            }
            catch (InvalidProductException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AppException($"Error al eliminar relación producto-proveedor: {ex.Message}", ex);
            }
        }

        public void Dispose()
        {
            _unitOfWork?.Dispose();
        }
    }
}
