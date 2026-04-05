using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Distribuidora.Application.Interfaces.Repositories;
using Distribuidora.Domain.Entities;

namespace Distribuidora.Persistence.Repositories
{
    /// <summary>
    /// Repositorio para la entidad Supplier
    /// Implementa operaciones CRUD usando ADO.NET
    /// </summary>
    public class SupplierRepository : ISupplierRepository
    {
        private readonly string _connectionString;

        public SupplierRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Obtiene todos los proveedores activos
        /// </summary>
        public async Task<IEnumerable<Supplier>> GetAllAsync()
        {
            var suppliers = new List<Supplier>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "dbo.sp_Suppliers_GetAll";
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandTimeout = 30;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                suppliers.Add(MapSupplier(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener proveedores: {ex.Message}", ex);
            }

            return suppliers;
        }

        /// <summary>
        /// Obtiene un proveedor por su Id
        /// </summary>
        public async Task<Supplier?> GetByIdAsync(int id)
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
                            FROM dbo.Suppliers
                            WHERE Id = @Id";
                        command.CommandType = System.Data.CommandType.Text;
                        command.Parameters.AddWithValue("@Id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return MapSupplier(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al obtener proveedor por Id: {ex.Message}", ex);
            }

            return null;
        }

        /// <summary>
        /// Agrega un nuevo proveedor
        /// </summary>
        public async Task<int> AddAsync(Supplier supplier)
        {
            if (supplier == null)
                throw new ArgumentNullException(nameof(supplier));

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            INSERT INTO dbo.Suppliers (Name, Description, Active, CreatedAt, UpdatedAt)
                            VALUES (@Name, @Description, @Active, @CreatedAt, @UpdatedAt);
                            SELECT SCOPE_IDENTITY();";
                        command.CommandType = System.Data.CommandType.Text;

                        command.Parameters.AddWithValue("@Name", supplier.Name ?? string.Empty);
                        command.Parameters.AddWithValue("@Description", supplier.Description ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Active", supplier.Active);
                        command.Parameters.AddWithValue("@CreatedAt", supplier.CreatedAt);
                        command.Parameters.AddWithValue("@UpdatedAt", supplier.UpdatedAt);

                        var result = await command.ExecuteScalarAsync();
                        return Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al agregar proveedor: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Actualiza un proveedor existente
        /// </summary>
        public async Task<bool> UpdateAsync(Supplier supplier)
        {
            if (supplier == null)
                throw new ArgumentNullException(nameof(supplier));

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            UPDATE dbo.Suppliers
                            SET Name = @Name, Description = @Description, Active = @Active, UpdatedAt = @UpdatedAt
                            WHERE Id = @Id";
                        command.CommandType = System.Data.CommandType.Text;

                        command.Parameters.AddWithValue("@Id", supplier.Id);
                        command.Parameters.AddWithValue("@Name", supplier.Name ?? string.Empty);
                        command.Parameters.AddWithValue("@Description", supplier.Description ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Active", supplier.Active);
                        command.Parameters.AddWithValue("@UpdatedAt", supplier.UpdatedAt);

                        var rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al actualizar proveedor: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Elimina (desactiva) un proveedor
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
                            UPDATE dbo.Suppliers
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
                throw new InvalidOperationException($"Error al eliminar proveedor: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Mapea un SqlDataReader a una entidad Supplier
        /// </summary>
        private static Supplier MapSupplier(System.Data.SqlClient.SqlDataReader reader)
        {
            return new Supplier
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
