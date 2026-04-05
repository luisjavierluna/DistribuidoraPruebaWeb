using Distribuidora.Persistence.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Registrar MigrationService para inyección de dependencias
builder.Services.AddScoped<IMigrationService, MigrationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

// Ejecutar migraciones de base de datos al iniciar la aplicación
using (var scope = app.Services.CreateScope())
{
    var migrationService = scope.ServiceProvider.GetRequiredService<IMigrationService>();
    var migrationResult = await migrationService.ExecuteMigrationsAsync();
    
    Console.WriteLine($"\n{'='*60}");
    Console.WriteLine("RESULTADO DE MIGRACIONES DE BASE DE DATOS:");
    Console.WriteLine($"{'='*60}");
    Console.WriteLine($"Éxito: {migrationResult.Success}");
    Console.WriteLine($"Scripts ejecutados: {migrationResult.ScriptsExecuted}");
    Console.WriteLine($"Scripts fallidos: {migrationResult.ScriptsFailed}");
    Console.WriteLine($"Mensaje: {migrationResult.Message}");
    Console.WriteLine($"{'='*60}\n");
    
    if (!migrationResult.Success)
    {
        Console.WriteLine("⚠ ADVERTENCIA: Hubo errores durante las migraciones");
    }
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
