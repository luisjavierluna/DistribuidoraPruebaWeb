using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Distribuidora.Application.Interfaces.Repositories;
using Distribuidora.Domain.Entities;

namespace Distribuidora.Persistence.Repositories
{
    /// <summary>
    /// Repositorio para la entidad ProductType
    /// Implementa operaciones CRUD usando ADO.NET
    /// </summary>
    public class ProductTypeRepository : IProductTypeRepository
    {
        private readonly string _connectionString;

        public ProductTypeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Obtiene todos los tipos de producto activos
        /// </summary>
        public async Task<IEnumerable<ProductType>> GetAllAsync()
        {
            var productTypes = new List<ProductType>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "dbo.sp_ProductTypes_GetAll";
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandTimeout = 30;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                productTypes.Add(MapProductType(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener tipos de producto: {ex.Message}", ex);
            }

            return productTypes;
        }

        /// <summary>
        /// Obtiene un tipo de producto por su Id
        /// </summary>
        public async Task<ProductType?> GetByIdAsync(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            SELECT Id, Name, Description, Active, CreatedAt, UpdatedAt
                            FROM dbo.ProductTypes
                            WHERE Id = @Id";
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.AddWithValue("@Id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return MapProductType(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener tipo de producto por Id: {ex.Message}", ex);
            }

            return null;
        }

        /// <summary>
        /// Agrega un nuevo tipo de producto
        /// </summary>
        public async Task<int> AddAsync(ProductType productType)
        {
            if (productType == null)
                throw new ArgumentNullException(nameof(productType));

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            INSERT INTO dbo.ProductTypes (Name, Description, Active, CreatedAt, UpdatedAt)
                            VALUES (@Name, @Description, @Active, @CreatedAt, @UpdatedAt);
                            SELECT SCOPE_IDENTITY();";
                        command.CommandType = System.Data.CommandType.Text;

                        command.Parameters.AddWithValue("@Name", productType.Name ?? string.Empty);
                        command.Parameters.AddWithValue("@Description", productType.Description ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Active", productType.Active);
                        command.Parameters.AddWithValue("@CreatedAt", productType.CreatedAt);
                        command.Parameters.AddWithValue("@UpdatedAt", productType.UpdatedAt);

                        var result = await command.ExecuteScalarAsync();
                        return Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al agregar tipo de producto: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Actualiza un tipo de producto existente
        /// </summary>
        public async Task<bool> UpdateAsync(ProductType productType)
        {
            if (productType == null)
                throw new ArgumentNullException(nameof(productType));

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            UPDATE dbo.ProductTypes
                            SET Name = @Name, Description = @Description, Active = @Active, UpdatedAt = @UpdatedAt
                            WHERE Id = @Id";
                        command.CommandType = System.Data.CommandType.Text;

                        command.Parameters.AddWithValue("@Id", productType.Id);
                        command.Parameters.AddWithValue("@Name", productType.Name ?? string.Empty);
                        command.Parameters.AddWithValue("@Description", productType.Description ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Active", productType.Active);
                        command.Parameters.AddWithValue("@UpdatedAt", productType.UpdatedAt);

                        var rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al actualizar tipo de producto: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Elimina (desactiva) un tipo de producto
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
                        command.CommandText = @"
                            UPDATE dbo.ProductTypes
                            SET Active = 0, UpdatedAt = @UpdatedAt
                            WHERE Id = @Id";
                        command.CommandType = System.Data.CommandType.Text;

                        command.Parameters.AddWithValue("@Id", id);
                        command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

                        var rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al eliminar tipo de producto: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Mapea un SqlDataReader a una entidad ProductType
        /// </summary>
        private static ProductType MapProductType(System.Data.SqlClient.SqlDataReader reader)
        {
            return new ProductType
            {
                Id = (int)reader["Id"],
                Name = reader["Name"]?.ToString() ?? string.Empty,
                Description = reader["Description"]?.ToString(),
                Active = (bool)reader["Active"],
                CreatedAt = (DateTime)reader["CreatedAt"],
                UpdatedAt = (DateTime)reader["UpdatedAt"]
            };
        }
    }
}
