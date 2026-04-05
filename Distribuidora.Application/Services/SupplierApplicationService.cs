using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Distribuidora.Application.DTOs;
using Distribuidora.Application.Exceptions;
using Distribuidora.Application.Interfaces.Repositories;
using Distribuidora.Domain.Entities;
using AppException = Distribuidora.Application.Exceptions.ApplicationException;

namespace Distribuidora.Application.Services
{
    /// <summary>
    /// Servicio de aplicación para Proveedores
    /// Orquesta lógica de negocio y valida operaciones
    /// </summary>
    public class SupplierApplicationService : IDisposable
    {
        private readonly IUnitOfWork _unitOfWork;

        public SupplierApplicationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Obtiene todos los proveedores activos
        /// </summary>
        public async Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync()
        {
            try
            {
                var suppliers = await _unitOfWork.Suppliers.GetAllAsync();
                return MapToSupplierDtos(suppliers);
            }
            catch (Exception ex)
            {
                throw new AppException($"Error al obtener proveedores: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene un proveedor por su Id
        /// </summary>
        public async Task<SupplierDto?> GetSupplierByIdAsync(int id)
        {
            if (id <= 0)
                throw new InvalidProductException("El Id debe ser mayor a 0");

            try
            {
                var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);
                if (supplier == null)
                    throw new SupplierNotFoundException(id);

                return MapToSupplierDto(supplier);
            }
            catch (Exception ex)
            {
                throw new AppException($"Error al obtener proveedor: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Crea un nuevo proveedor
        /// </summary>
        public async Task<SupplierDto> CreateSupplierAsync(CreateSupplierDto createDto)
        {
            if (createDto == null)
                throw new ArgumentNullException(nameof(createDto));

            ValidateSupplierDto(createDto.Name);

            try
            {
                var supplier = new Supplier
                {
                    Name = createDto.Name,
                    Description = createDto.Description,
                    Active = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var newId = await _unitOfWork.Suppliers.AddAsync(supplier);
                supplier.Id = newId;

                await _unitOfWork.SaveChangesAsync();
                return MapToSupplierDto(supplier);
            }
            catch (Exception ex)
            {
                throw new AppException($"Error al crear proveedor: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Actualiza un proveedor existente
        /// </summary>
        public async Task<SupplierDto> UpdateSupplierAsync(UpdateSupplierDto updateDto)
        {
            if (updateDto == null)
                throw new ArgumentNullException(nameof(updateDto));

            if (updateDto.Id <= 0)
                throw new InvalidProductException("El Id debe ser mayor a 0");

            ValidateSupplierDto(updateDto.Name);

            try
            {
                var supplier = await _unitOfWork.Suppliers.GetByIdAsync(updateDto.Id);
                if (supplier == null)
                    throw new SupplierNotFoundException(updateDto.Id);

                supplier.Name = updateDto.Name;
                supplier.Description = updateDto.Description;
                supplier.UpdatedAt = DateTime.UtcNow;

                var updated = await _unitOfWork.Suppliers.UpdateAsync(supplier);
                if (!updated)
                    throw new AppException("No se pudo actualizar el proveedor");

                await _unitOfWork.SaveChangesAsync();
                return MapToSupplierDto(supplier);
            }
            catch (Exception ex)
            {
                throw new AppException($"Error al actualizar proveedor: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Elimina un proveedor
        /// </summary>
        public async Task<bool> DeleteSupplierAsync(int id)
        {
            if (id <= 0)
                throw new InvalidProductException("El Id debe ser mayor a 0");

            try
            {
                var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);
                if (supplier == null)
                    throw new SupplierNotFoundException(id);

                var deleted = await _unitOfWork.Suppliers.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();
                return deleted;
            }
            catch (Exception ex)
            {
                throw new AppException($"Error al eliminar proveedor: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Valida los datos de un proveedor
        /// </summary>
        private void ValidateSupplierDto(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidProductException("El nombre es requerido");

            if (name.Length > 255)
                throw new InvalidProductException("El nombre no puede exceder 255 caracteres");
        }

        /// <summary>
        /// Mapea una entidad Supplier a SupplierDto
        /// </summary>
        private SupplierDto MapToSupplierDto(Supplier supplier)
        {
            return new SupplierDto
            {
                Id = supplier.Id,
                Name = supplier.Name ?? string.Empty,
                Description = supplier.Description,
                Active = supplier.Active,
                CreatedAt = supplier.CreatedAt,
                UpdatedAt = supplier.UpdatedAt
            };
        }

        /// <summary>
        /// Mapea una colección de Suppliers a SupplierDtos
        /// </summary>
        private IEnumerable<SupplierDto> MapToSupplierDtos(IEnumerable<Supplier> suppliers)
        {
            foreach (var supplier in suppliers)
            {
                yield return MapToSupplierDto(supplier);
            }
        }

        /// <summary>
        /// Libera recursos
        /// </summary>
        public void Dispose()
        {
            _unitOfWork?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
