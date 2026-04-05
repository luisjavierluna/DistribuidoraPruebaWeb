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
    /// Servicio de aplicación para Tipos de Producto
    /// Orquesta lógica de negocio y valida operaciones
    /// </summary>
    public class ProductTypeApplicationService : IDisposable
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductTypeApplicationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Obtiene todos los tipos de producto activos
        /// </summary>
        public async Task<IEnumerable<ProductTypeDto>> GetAllProductTypesAsync()
        {
            try
            {
                var productTypes = await _unitOfWork.ProductTypes.GetAllAsync();
                return MapToProductTypeDtos(productTypes);
            }
            catch (Exception ex)
            {
                throw new AppException($"Error al obtener tipos de producto: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene un tipo de producto por su Id
        /// </summary>
        public async Task<ProductTypeDto?> GetProductTypeByIdAsync(int id)
        {
            if (id <= 0)
                throw new InvalidProductException("El Id debe ser mayor a 0");

            try
            {
                var productType = await _unitOfWork.ProductTypes.GetByIdAsync(id);
                if (productType == null)
                    throw new ProductTypeNotFoundException(id);

                return MapToProductTypeDto(productType);
            }
            catch (Exception ex)
            {
                throw new AppException($"Error al obtener tipo de producto: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Crea un nuevo tipo de producto
        /// </summary>
        public async Task<ProductTypeDto> CreateProductTypeAsync(CreateProductTypeDto createDto)
        {
            if (createDto == null)
                throw new ArgumentNullException(nameof(createDto));

            ValidateProductTypeDto(createDto.Name);

            try
            {
                var productType = new ProductType
                {
                    Name = createDto.Name,
                    Description = createDto.Description,
                    Active = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var newId = await _unitOfWork.ProductTypes.AddAsync(productType);
                productType.Id = newId;

                await _unitOfWork.SaveChangesAsync();
                return MapToProductTypeDto(productType);
            }
            catch (Exception ex)
            {
                throw new AppException($"Error al crear tipo de producto: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Actualiza un tipo de producto existente
        /// </summary>
        public async Task<ProductTypeDto> UpdateProductTypeAsync(UpdateProductTypeDto updateDto)
        {
            if (updateDto == null)
                throw new ArgumentNullException(nameof(updateDto));

            if (updateDto.Id <= 0)
                throw new InvalidProductException("El Id debe ser mayor a 0");

            ValidateProductTypeDto(updateDto.Name);

            try
            {
                var productType = await _unitOfWork.ProductTypes.GetByIdAsync(updateDto.Id);
                if (productType == null)
                    throw new ProductTypeNotFoundException(updateDto.Id);

                productType.Name = updateDto.Name;
                productType.Description = updateDto.Description;
                productType.UpdatedAt = DateTime.UtcNow;

                var updated = await _unitOfWork.ProductTypes.UpdateAsync(productType);
                if (!updated)
                    throw new AppException("No se pudo actualizar el tipo de producto");

                await _unitOfWork.SaveChangesAsync();
                return MapToProductTypeDto(productType);
            }
            catch (Exception ex)
            {
                throw new AppException($"Error al actualizar tipo de producto: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Elimina un tipo de producto
        /// </summary>
        public async Task<bool> DeleteProductTypeAsync(int id)
        {
            if (id <= 0)
                throw new InvalidProductException("El Id debe ser mayor a 0");

            try
            {
                var productType = await _unitOfWork.ProductTypes.GetByIdAsync(id);
                if (productType == null)
                    throw new ProductTypeNotFoundException(id);

                var deleted = await _unitOfWork.ProductTypes.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();
                return deleted;
            }
            catch (Exception ex)
            {
                throw new AppException($"Error al eliminar tipo de producto: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Valida los datos de un tipo de producto
        /// </summary>
        private void ValidateProductTypeDto(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidProductException("El nombre es requerido");

            if (name.Length > 255)
                throw new InvalidProductException("El nombre no puede exceder 255 caracteres");
        }

        /// <summary>
        /// Mapea una entidad ProductType a ProductTypeDto
        /// </summary>
        private ProductTypeDto MapToProductTypeDto(ProductType productType)
        {
            return new ProductTypeDto
            {
                Id = productType.Id,
                Name = productType.Name ?? string.Empty,
                Description = productType.Description,
                Active = productType.Active,
                CreatedAt = productType.CreatedAt,
                UpdatedAt = productType.UpdatedAt
            };
        }

        /// <summary>
        /// Mapea una colección de ProductTypes a ProductTypeDtos
        /// </summary>
        private IEnumerable<ProductTypeDto> MapToProductTypeDtos(IEnumerable<ProductType> productTypes)
        {
            foreach (var productType in productTypes)
            {
                yield return MapToProductTypeDto(productType);
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
