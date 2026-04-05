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
    /// Servicio de aplicación para Productos
    /// Orquesta lógica de negocio y valida operaciones
    /// </summary>
    public class ProductApplicationService : IDisposable
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductApplicationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Obtiene todos los productos activos
        /// </summary>
        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            try
            {
                var products = await _unitOfWork.Products.GetAllAsync();
                return MapToProductDtos(products);
            }
            catch (Exception ex)
            {
                throw new AppException($"Error al obtener productos: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene un producto por su Id
        /// </summary>
        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            if (id <= 0)
                throw new InvalidProductException("El Id debe ser mayor a 0");

            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(id);
                if (product == null)
                    throw new InvalidProductException($"Producto con Id {id} no encontrado");

                return MapToProductDto(product);
            }
            catch (InvalidProductException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AppException($"Error al obtener producto: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene productos por tipo
        /// </summary>
        public async Task<IEnumerable<ProductDto>> GetProductsByTypeAsync(int productTypeId)
        {
            if (productTypeId <= 0)
                throw new InvalidProductException("El ProductTypeId debe ser mayor a 0");

            try
            {
                // Verificar que el tipo existe
                var productType = await _unitOfWork.ProductTypes.GetByIdAsync(productTypeId);
                if (productType == null)
                    throw new ProductTypeNotFoundException(productTypeId);

                var products = await _unitOfWork.Products.GetByProductTypeAsync(productTypeId);
                return MapToProductDtos(products);
            }
            catch (Exception ex)
            {
                throw new AppException($"Error al obtener productos por tipo: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Crea un nuevo producto
        /// </summary>
        public async Task<ProductDto> CreateProductAsync(CreateProductDto createDto)
        {
            if (createDto == null)
                throw new ArgumentNullException(nameof(createDto));

            ValidateProductDto(createDto.Code, createDto.Name);

            try
            {
                // Verificar que el tipo de producto existe
                var productType = await _unitOfWork.ProductTypes.GetByIdAsync(createDto.ProductTypeId);
                if (productType == null)
                    throw new ProductTypeNotFoundException(createDto.ProductTypeId);

                var product = new Product
                {
                    Code = createDto.Code,
                    Name = createDto.Name,
                    ProductTypeId = createDto.ProductTypeId,
                    Price = createDto.Price,
                    Active = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var newId = await _unitOfWork.Products.AddAsync(product);
                product.Id = newId;

                await _unitOfWork.SaveChangesAsync();
                return MapToProductDto(product);
            }
            catch (Exception ex)
            {
                throw new AppException($"Error al crear producto: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Actualiza un producto existente
        /// </summary>
        public async Task<ProductDto> UpdateProductAsync(UpdateProductDto updateDto)
        {
            if (updateDto == null)
                throw new ArgumentNullException(nameof(updateDto));

            if (updateDto.Id <= 0)
                throw new InvalidProductException("El Id debe ser mayor a 0");

            ValidateProductDto(updateDto.Code, updateDto.Name);

            try
            {
                // Verificar que el producto existe
                var product = await _unitOfWork.Products.GetByIdAsync(updateDto.Id);
                if (product == null)
                    throw new InvalidProductException("Producto no encontrado");

                // Verificar que el tipo de producto existe
                var productType = await _unitOfWork.ProductTypes.GetByIdAsync(updateDto.ProductTypeId);
                if (productType == null)
                    throw new ProductTypeNotFoundException(updateDto.ProductTypeId);

                // Actualizar propiedades
                product.Code = updateDto.Code;
                product.Name = updateDto.Name;
                product.ProductTypeId = updateDto.ProductTypeId;
                product.Price = updateDto.Price;
                product.UpdatedAt = DateTime.UtcNow;

                var updated = await _unitOfWork.Products.UpdateAsync(product);
                if (!updated)
                    throw new InvalidProductException($"No se pudo actualizar el producto");

                await _unitOfWork.SaveChangesAsync();
                return MapToProductDto(product);
            }
            catch (Exception ex)
            {
                throw new AppException($"Error al actualizar producto: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Elimina un producto
        /// </summary>
        public async Task<bool> DeleteProductAsync(int id)
        {
            if (id <= 0)
                throw new InvalidProductException("El Id debe ser mayor a 0");

            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(id);
                if (product == null)
                    throw new InvalidProductException($"Producto con Id {id} no encontrado");

                var deleted = await _unitOfWork.Products.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();
                return deleted;
            }
            catch (Exception ex)
            {
                throw new AppException($"Error al eliminar producto: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Valida los datos de un producto
        /// </summary>
        private void ValidateProductDto(string? code, string? name)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new InvalidProductException("El código es requerido");

            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidProductException("El nombre es requerido");

            if (code.Length > 100)
                throw new InvalidProductException("El código no puede exceder 100 caracteres");

            if (name.Length > 255)
                throw new InvalidProductException("El nombre no puede exceder 255 caracteres");
        }

        /// <summary>
        /// Mapea una entidad Product a ProductDto
        /// </summary>
        private ProductDto MapToProductDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Code = product.Code ?? string.Empty,
                Name = product.Name ?? string.Empty,
                ProductTypeId = product.ProductTypeId,
                ProductTypeName = product.ProductType?.Name,
                Price = product.Price,
                Active = product.Active,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }

        /// <summary>
        /// Mapea una colección de Products a ProductDtos
        /// </summary>
        private IEnumerable<ProductDto> MapToProductDtos(IEnumerable<Product> products)
        {
            foreach (var product in products)
            {
                yield return MapToProductDto(product);
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
