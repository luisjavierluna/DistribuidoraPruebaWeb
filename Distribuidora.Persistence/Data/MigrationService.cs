using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Distribuidora.Persistence.Data
{
    /// <summary>
    /// Servicio para ejecutar migraciones de la base de datos
    /// </summary>
    public class MigrationService : IMigrationService
    {
        private readonly string _connectionString;
        private readonly string _migrationsPath;
        private readonly string _storedProceduresPath;

        public MigrationService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new ArgumentNullException(nameof(configuration), "DefaultConnection no configurada en appsettings.json");
            
            // Detectar rutas de scripts - buscar desde la raíz del proyecto
            var rootDirectory = FindSolutionRoot(Directory.GetCurrentDirectory());
            
            _migrationsPath = Path.Combine(rootDirectory, "Distribuidora.Persistence", "Database", "Migrations");
            _storedProceduresPath = Path.Combine(rootDirectory, "Distribuidora.Persistence", "Database", "StoredProcedures");

            // Fallback si no encuentra en la ruta esperada
            if (!Directory.Exists(_migrationsPath))
            {
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                _migrationsPath = Path.Combine(baseDirectory, "..", "..", "..", "..", "Distribuidora.Persistence", "Database", "Migrations");
                _storedProceduresPath = Path.Combine(baseDirectory, "..", "..", "..", "..", "Distribuidora.Persistence", "Database", "StoredProcedures");
            }
        }

        /// <summary>
        /// Encuentra la carpeta raíz del proyecto buscando el archivo .sln
        /// </summary>
        private string FindSolutionRoot(string startPath)
        {
            var currentPath = startPath;
            
            while (!string.IsNullOrEmpty(currentPath))
            {
                if (File.Exists(Path.Combine(currentPath, "DistribuidoraPruebaWeb.sln")))
                {
                    return currentPath;
                }
                
                var parentPath = Directory.GetParent(currentPath)?.FullName;
                if (parentPath == currentPath) // Llegó a la raíz del disco
                    break;
                    
                currentPath = parentPath;
            }
            
            // Si no encuentra el .sln, retorna el directorio actual
            return Directory.GetCurrentDirectory();
        }

        /// <summary>
        /// Ejecuta todas las migraciones pendientes en orden
        /// </summary>
        public async Task<MigrationResult> ExecuteMigrationsAsync()
        {
            var result = new MigrationResult
            {
                Success = true,
                ScriptsExecuted = 0,
                ScriptsFailed = 0
            };

            try
            {
                // Debug: mostrar rutas y conexión
                // Console.WriteLine($"\n Rutas de scripts:");
                // Console.WriteLine($"   Migraciones: {_migrationsPath}");
                // Console.WriteLine($"   SPs: {_storedProceduresPath}");
                // Console.WriteLine($"   Archivos encontrados: {GetScriptFiles(_migrationsPath).Count + GetScriptFiles(_storedProceduresPath).Count}");
                // Console.WriteLine($"\n Cadena de conexión:");
                // Console.WriteLine($"   {_connectionString}\n");

                // Verificar si la base de datos existe
                var databaseExists = await DatabaseExistsAsync();
                
                if (!databaseExists)
                {
                    // Si no existe, ejecutar solo el script de creación de BD primero
                    var createDbScript = GetScriptFiles(_migrationsPath)
                        .FirstOrDefault(f => Path.GetFileName(f).StartsWith("001_"));
                    
                    if (createDbScript != null)
                    {
                        try
                        {
                            await ExecuteScriptAsync(createDbScript, useTarget: false); // Conectarse a master
                            await LogMigrationAsync(Path.GetFileName(createDbScript));
                            result.ScriptsExecuted++;
                        }
                        catch (Exception ex)
                        {
                            result.ScriptsFailed++;
                            result.Success = false;
                            result.Message = $"✗ Error creando base de datos: {ex.Message}";
                            return result;
                        }
                    }
                }

                // Obtener lista de scripts ejecutados
                var executedScripts = await GetExecutedScriptsAsync();

                // Obtener lista de scripts pendientes (Migrations)
                var migrationScripts = GetScriptFiles(_migrationsPath)
                    .Where(f => !executedScripts.Contains(Path.GetFileName(f)))
                    .OrderBy(f => f)
                    .ToList();

                // Obtener lista de scripts pendientes (StoredProcedures)
                var storedProcedureScripts = GetScriptFiles(_storedProceduresPath)
                    .Where(f => !executedScripts.Contains(Path.GetFileName(f)))
                    .OrderBy(f => f)
                    .ToList();

                var allScripts = migrationScripts.Concat(storedProcedureScripts).ToList();

                if (allScripts.Count == 0)
                {
                    result.Message = "Todas las migraciones ya han sido ejecutadas.";
                    return result;
                }

                // Ejecutar cada script
                foreach (var scriptPath in allScripts)
                {
                    var scriptName = Path.GetFileName(scriptPath);
                    try
                    {
                        // Console.WriteLine($"   ⏳ Ejecutando: {scriptName}");
                        await ExecuteScriptAsync(scriptPath);
                        await LogMigrationAsync(scriptName);
                        result.ScriptsExecuted++;
                        // Console.WriteLine($"   ✓ {scriptName} - OK");
                    }
                    catch (Exception ex)
                    {
                        result.ScriptsFailed++;
                        result.Success = false;
                        // Console.WriteLine($"   ✗ {scriptName} - ERROR: {ex.Message}");
                        result.Message += $"\n✗ Error en {scriptName}: {ex.Message}";
                    }
                }

                result.Message = $"✓ Migraciones completadas. Scripts ejecutados: {result.ScriptsExecuted}, Errores: {result.ScriptsFailed}";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"✗ Error fatal en migraciones: {ex.Message}";
            }

            return result;
        }

        /// <summary>
        /// Verifica si la base de datos existe
        /// </summary>
        private async Task<bool> DatabaseExistsAsync()
        {
            try
            {
                var connectionStringBuilder = new SqlConnectionStringBuilder(_connectionString);
                var masterConnectionString = _connectionString.Replace(connectionStringBuilder.InitialCatalog, "master");
                
                // Console.WriteLine($"      Verificando BD (conectando a 'master')...");
                // Console.WriteLine($"      Conexión: {masterConnectionString}");

                using (var connection = new SqlConnection(masterConnectionString))
                {
                    await connection.OpenAsync();
                    
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = $"SELECT 1 FROM sys.databases WHERE name = '{connectionStringBuilder.InitialCatalog}'";
                    
                    var result = await cmd.ExecuteScalarAsync();
                    bool exists = result != null;
                    // Console.WriteLine($"   BD '{connectionStringBuilder.InitialCatalog}' existe: {exists}");
                    return exists;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   No se pudo verificar BD: {ex.Message}");
                // Si no podemos verificar, asumimos que existe
                return true;
            }
        }

        /// <summary>
        /// Obtiene la lista de scripts que ya han sido ejecutados
        /// </summary>
        private async Task<List<string>> GetExecutedScriptsAsync()
        {
            var executedScripts = new List<string>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Verificar si la tabla Migrations existe
                    var checkTableCmd = connection.CreateCommand();
                    checkTableCmd.CommandText = @"
                        IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES 
                                   WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Migrations')
                        SELECT 1
                    ";
                    
                    var result = await checkTableCmd.ExecuteScalarAsync();
                    
                    if (result != null)
                    {
                        var cmd = connection.CreateCommand();
                        cmd.CommandText = "SELECT MigrationName FROM dbo.Migrations ORDER BY ExecutedAt ASC";
                        
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                executedScripts.Add(reader["MigrationName"].ToString());
                            }
                        }
                    }
                }
            }
            catch
            {
                // Si algo falla, asumimos que no hay registros
                executedScripts.Clear();
            }

            return executedScripts;
        }

        /// <summary>
        /// Ejecuta un archivo SQL script
        /// </summary>
        private async Task ExecuteScriptAsync(string scriptPath, bool useTarget = true)
        {
            if (!File.Exists(scriptPath))
                throw new FileNotFoundException($"Script no encontrado: {scriptPath}");

            var scriptContent = await File.ReadAllTextAsync(scriptPath);
            
            // Si es el script de creación de DB, conectarse a master
            var connectionString = useTarget ? _connectionString : GetMasterConnectionString();
            var targetDb = useTarget ? "DistribuidoraDb" : "master";

            try
            {
                // Console.WriteLine($"      Conectando a: {targetDb}");
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    // Console.WriteLine($"      Conexión abierta a {targetDb}");

                    // Dividir por GO para ejecutar en lotes
                    var batches = scriptContent.Split(new[] { "\nGO", "\r\nGO", "\ngo", "\r\ngo" }, StringSplitOptions.RemoveEmptyEntries);

                    int batchCount = 0;
                    foreach (var batch in batches)
                    {
                        if (string.IsNullOrWhiteSpace(batch))
                            continue;

                        batchCount++;
                        try
                        {
                            using (var command = connection.CreateCommand())
                            {
                                command.CommandText = batch.Trim();
                                command.CommandTimeout = 300; // 5 minutos
                                await command.ExecuteNonQueryAsync();
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidOperationException($"Error en batch {batchCount}: {ex.Message}", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error ejecutando script {Path.GetFileName(scriptPath)}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Registra el script ejecutado en la tabla Migrations
        /// </summary>
        private async Task LogMigrationAsync(string migrationName)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Primero, verificar si la tabla Migrations existe
                    using (var checkCommand = connection.CreateCommand())
                    {
                        checkCommand.CommandText = @"
                            IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES 
                                           WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Migrations')
                            BEGIN
                                CREATE TABLE dbo.Migrations (
                                    Id INT PRIMARY KEY IDENTITY(1,1),
                                    MigrationName NVARCHAR(255) NOT NULL UNIQUE,
                                    ExecutedAt DATETIME NOT NULL DEFAULT GETUTCDATE()
                                );
                            END
                        ";
                        await checkCommand.ExecuteNonQueryAsync();
                    }

                    // Ahora insertar el registro de migración
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            IF NOT EXISTS (SELECT 1 FROM dbo.Migrations WHERE MigrationName = @Name)
                            BEGIN
                                INSERT INTO dbo.Migrations (MigrationName, ExecutedAt)
                                VALUES (@Name, GETUTCDATE())
                            END
                        ";

                        command.Parameters.AddWithValue("@Name", migrationName);
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"No se pudo registrar la migración {migrationName}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene cadena de conexión a master
        /// </summary>
        private string GetMasterConnectionString()
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder(_connectionString);
            connectionStringBuilder.InitialCatalog = "master";
            return connectionStringBuilder.ConnectionString;
        }

        /// <summary>
        /// Obtiene la lista de archivos .sql en una carpeta
        /// </summary>
        private List<string> GetScriptFiles(string directoryPath)
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                    return new List<string>();

                return Directory
                    .GetFiles(directoryPath, "*.sql")
                    .OrderBy(f => f)
                    .ToList();
            }
            catch
            {
                return new List<string>();
            }
        }
    }
}
