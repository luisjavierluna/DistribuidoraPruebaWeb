using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Distribuidora.Application.Interfaces.Repositories;
using Distribuidora.Domain.Entities;

namespace Distribuidora.Persistence.Repositories
{
    /// <summary>
    /// Repositorio para la entidad Product
    /// Implementa operaciones CRUD usando ADO.NET y Stored Procedures
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly string _connectionString;

        public ProductRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Obtiene todos los productos activos
        /// </summary>
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await GetAllInternalAsync(null, null);
        }

        /// <summary>
        /// Obtiene todos los productos activos con filtros opcionales
        /// </summary>
        private async Task<IEnumerable<Product>> GetAllInternalAsync(string? code = null, int? productTypeId = null)
        {
            var products = new List<Product>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "dbo.sp_Products_GetAll";
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandTimeout = 30;

                        command.Parameters.AddWithValue("@Code", code ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ProductTypeId", productTypeId ?? (object)DBNull.Value);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                products.Add(MapProduct(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener productos: {ex.Message}", ex);
            }

            return products;
        }

        /// <summary>
        /// Obtiene un producto por su Id
        /// </summary>
        public async Task<Product?> GetByIdAsync(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "dbo.sp_Products_GetById";
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandTimeout = 30;

                        command.Parameters.AddWithValue("@Id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return MapProduct(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener producto por Id: {ex.Message}", ex);
            }

            return null;
        }

        /// <summary>
        /// Obtiene productos por tipo de producto
        /// </summary>
        public async Task<IEnumerable<Product>> GetByProductTypeAsync(int productTypeId)
        {
            return await GetAllInternalAsync(productTypeId: productTypeId);
        }

        /// <summary>
        /// Agrega un nuevo producto
        /// </summary>
        public async Task<int> AddAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "dbo.sp_Products_Insert";
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandTimeout = 30;

                        command.Parameters.AddWithValue("@Code", product.Code ?? string.Empty);
                        command.Parameters.AddWithValue("@Name", product.Name ?? string.Empty);
                        command.Parameters.AddWithValue("@ProductTypeId", product.ProductTypeId);
                        command.Parameters.AddWithValue("@Price", product.Price ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Active", product.Active);

                        var newIdParam = command.Parameters.Add("@NewId", System.Data.SqlDbType.Int);
                        newIdParam.Direction = System.Data.ParameterDirection.Output;

                        await command.ExecuteNonQueryAsync();
                        return (int)command.Parameters["@NewId"].Value;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al agregar producto: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Actualiza un producto existente
        /// </summary>
        public async Task<bool> UpdateAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "dbo.sp_Products_Update";
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandTimeout = 30;

                        command.Parameters.AddWithValue("@Id", product.Id);
                        command.Parameters.AddWithValue("@Code", product.Code ?? string.Empty);
                        command.Parameters.AddWithValue("@Name", product.Name ?? string.Empty);
                        command.Parameters.AddWithValue("@ProductTypeId", product.ProductTypeId);
                        command.Parameters.AddWithValue("@Price", product.Price ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Active", product.Active);

                        var rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al actualizar producto: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Elimina (desactiva) un producto
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "dbo.sp_Products_Delete";
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandTimeout = 30;

                        command.Parameters.AddWithValue("@Id", id);

                        var rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al eliminar producto: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Mapea un SqlDataReader a una entidad Product
        /// </summary>
        private static Product MapProduct(System.Data.SqlClient.SqlDataReader reader)
        {
            return new Product
            {
                Id = (int)reader["Id"],
                Code = reader["Code"]?.ToString() ?? string.Empty,
                Name = reader["Name"]?.ToString() ?? string.Empty,
                ProductTypeId = (int)reader["ProductTypeId"],
                Price = reader["Price"] is DBNull ? null : (decimal?)reader["Price"],
                Active = (bool)reader["Active"],
                CreatedAt = (DateTime)reader["CreatedAt"],
                UpdatedAt = (DateTime)reader["UpdatedAt"]
            };
        }
    }
}
