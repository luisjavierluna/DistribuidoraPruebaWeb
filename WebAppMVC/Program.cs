using Distribuidora.Persistence.Data;
using Distribuidora.Application.Interfaces.Repositories;
using Distribuidora.Application.Services;
using Distribuidora.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Get connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Registrar MigrationService para inyección de dependencias
builder.Services.AddScoped<IMigrationService, MigrationService>();

// Registrar Repositories (patrón Repository)
builder.Services.AddScoped<IProductRepository>(sp => new ProductRepository(connectionString));
builder.Services.AddScoped<ISupplierRepository>(sp => new SupplierRepository(connectionString));
builder.Services.AddScoped<IProductTypeRepository>(sp => new ProductTypeRepository(connectionString));
builder.Services.AddScoped<IProductSupplierRepository>(sp => new ProductSupplierRepository(connectionString));

// Registrar UnitOfWork
builder.Services.AddScoped<IUnitOfWork>(sp => new UnitOfWork(connectionString));

// Registrar Application Services
builder.Services.AddScoped<ProductApplicationService>();
builder.Services.AddScoped<SupplierApplicationService>();
builder.Services.AddScoped<ProductTypeApplicationService>();
builder.Services.AddScoped<ProductSupplierApplicationService>();

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
