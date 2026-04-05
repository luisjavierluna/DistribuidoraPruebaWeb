using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Distribuidora.Persistence.Data;

namespace WebAppMVC.Controllers
{
    /// <summary>
    /// Controller para gestionar operaciones de base de datos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DatabaseController : ControllerBase
    {
        private readonly IMigrationService _migrationService;

        public DatabaseController(IMigrationService migrationService)
        {
            _migrationService = migrationService;
        }

        /// <summary>
        /// Ejecuta todas las migraciones de base de datos pendientes
        /// GET: /api/database/migrate
        /// </summary>
        /// <returns>Resultado de la ejecución de migraciones</returns>
        [HttpGet("migrate")]
        public async Task<ActionResult<MigrationResult>> Migrate()
        {
            try
            {
                var result = await _migrationService.ExecuteMigrationsAsync();
                
                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = "Error executing migrations", error = ex.Message });
            }
        }
    }
}
