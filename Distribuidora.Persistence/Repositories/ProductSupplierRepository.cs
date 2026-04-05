using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Distribuidora.Application.Interfaces.Repositories;
using Distribuidora.Domain.Entities;

namespace Distribuidora.Persistence.Repositories
{
    /// <summary>
    /// Repositorio para la entidad ProductSupplier (relación N:N)
    /// Implementa operaciones CRUD usando ADO.NET y Stored Procedures
    /// </summary>
    public class ProductSupplierRepository : IProductSupplierRepository
    {
        private readonly string _connectionString;

        public ProductSupplierRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Obtiene todos los proveedores de un producto
        /// </summary>
        public async Task<IEnumerable<ProductSupplier>> GetByProductIdAsync(int productId)
        {
            var productSuppliers = new List<ProductSupplier>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "dbo.sp_ProductSuppliers_GetByProductId";
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandTimeout = 30;

                        command.Parameters.AddWithValue("@ProductId", productId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                productSuppliers.Add(MapProductSupplier(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener proveedores del producto: {ex.Message}", ex);
            }

            return productSuppliers;
        }

        /// <summary>
        /// Obtiene un ProductSupplier por su Id
        /// </summary>
        public async Task<ProductSupplier?> GetByIdAsync(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            SELECT Id, ProductId, SupplierId, SupplierProductCode, Cost, Active, CreatedAt, UpdatedAt
                            FROM dbo.ProductSuppliers
                            WHERE Id = @Id";
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.AddWithValue("@Id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return MapProductSupplier(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener ProductSupplier por Id: {ex.Message}", ex);
            }

            return null;
        }

        /// <summary>
        /// Agrega una nueva relación ProductSupplier
        /// </summary>
        public async Task<int> AddAsync(ProductSupplier productSupplier)
        {
            if (productSupplier == null)
                throw new ArgumentNullException(nameof(productSupplier));

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "dbo.sp_ProductSuppliers_Insert";
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandTimeout = 30;

                        command.Parameters.AddWithValue("@ProductId", productSupplier.ProductId);
                        command.Parameters.AddWithValue("@SupplierId", productSupplier.SupplierId);
                        command.Parameters.AddWithValue("@SupplierProductCode", productSupplier.SupplierProductCode ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Cost", productSupplier.Cost);
                        command.Parameters.AddWithValue("@Active", productSupplier.Active);

                        var newIdParam = command.Parameters.Add("@NewId", System.Data.SqlDbType.Int);
                        newIdParam.Direction = System.Data.ParameterDirection.Output;

                        await command.ExecuteNonQueryAsync();
                        return (int)command.Parameters["@NewId"].Value;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al agregar ProductSupplier: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Actualiza una relación ProductSupplier existente
        /// </summary>
        public async Task<bool> UpdateAsync(ProductSupplier productSupplier)
        {
            if (productSupplier == null)
                throw new ArgumentNullException(nameof(productSupplier));

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            UPDATE dbo.ProductSuppliers
                            SET ProductId = @ProductId, SupplierId = @SupplierId, 
                                SupplierProductCode = @SupplierProductCode, Cost = @Cost, 
                                Active = @Active, UpdatedAt = @UpdatedAt
                            WHERE Id = @Id";
                        command.CommandType = System.Data.CommandType.Text;

                        command.Parameters.AddWithValue("@Id", productSupplier.Id);
                        command.Parameters.AddWithValue("@ProductId", productSupplier.ProductId);
                        command.Parameters.AddWithValue("@SupplierId", productSupplier.SupplierId);
                        command.Parameters.AddWithValue("@SupplierProductCode", productSupplier.SupplierProductCode ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Cost", productSupplier.Cost);
                        command.Parameters.AddWithValue("@Active", productSupplier.Active);
                        command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

                        var rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al actualizar ProductSupplier: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Elimina (desactiva) una relación ProductSupplier
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
                        command.CommandText = "dbo.sp_ProductSuppliers_Delete";
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
                throw new InvalidOperationException($"Error al eliminar ProductSupplier: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Mapea un SqlDataReader a una entidad ProductSupplier
        /// </summary>
        private static ProductSupplier MapProductSupplier(System.Data.SqlClient.SqlDataReader reader)
        {
            return new ProductSupplier
            {
                Id = (int)reader["Id"],
                ProductId = (int)reader["ProductId"],
                SupplierId = (int)reader["SupplierId"],
                SupplierProductCode = reader["SupplierProductCode"]?.ToString(),
                Cost = (decimal)reader["Cost"],
                Active = (bool)reader["Active"],
                CreatedAt = (DateTime)reader["CreatedAt"],
                UpdatedAt = (DateTime)reader["UpdatedAt"]
            };
        }
    }
}
