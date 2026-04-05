using System.Threading.Tasks;

namespace Distribuidora.Persistence.Data
{
    /// <summary>
    /// Interfaz para gestionar las migraciones de base de datos
    /// </summary>
    public interface IMigrationService
    {
        /// <summary>
        /// Ejecuta todas las migraciones pendientes
        /// </summary>
        /// <returns>Resultado del proceso de migración</returns>
        Task<MigrationResult> ExecuteMigrationsAsync();
    }

    /// <summary>
    /// Resultado de la ejecución de migraciones
    /// </summary>
    public class MigrationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int ScriptsExecuted { get; set; }
        public int ScriptsFailed { get; set; }
    }
}
