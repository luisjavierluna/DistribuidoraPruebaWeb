using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebAppMVC.Models;
using Distribuidora.Application.Services;
using Distribuidora.Application.DTOs;
using Distribuidora.Application.Exceptions;

namespace WebAppMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProductApplicationService _productService;
        private readonly ProductTypeApplicationService _productTypeService;

        public HomeController(
            ILogger<HomeController> logger,
            ProductApplicationService productService,
            ProductTypeApplicationService productTypeService)
        {
            _logger = logger;
            _productService = productService;
            _productTypeService = productTypeService;
        }

        public async Task<IActionResult> Index(string clave = "", string tipoProducto = "")
        {
            try
            {
                var productosDtos = await _productService.GetAllProductsAsync();
                var productosFiltrados = productosDtos.ToList();

                if (!string.IsNullOrEmpty(clave))
                {
                    productosFiltrados = productosFiltrados.Where(p => p.Code.Contains(clave, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                if (!string.IsNullOrEmpty(tipoProducto) && int.TryParse(tipoProducto, out int typeId))
                {
                    productosFiltrados = productosFiltrados.Where(p => p.ProductTypeId == typeId).ToList();
                }

                // Mapeamos DTOs a modelos de vista
                var productos = productosDtos.Select(p => new Producto
                {
                    Id = p.Id,
                    Clave = p.Code,
                    Nombre = p.Name,
                    TipoProducto = p.ProductTypeName ?? "Sin tipo",
                    EsActivo = p.Active,
                    Precio = p.Price ?? 0,
                    Proveedores = new List<Proveedor>()
                }).ToList();

                return View(productos);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener productos: {ex.Message}");
                return View(new List<Producto>());
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                if (id == 0)
                {
                    // Nuevo producto
                    var nuevoProducto = new Producto { Id = 0 };
                    ViewBag.TiposProducto = await ObtenerTiposProducto();
                    return View(nuevoProducto);
                }

                var productoDto = await _productService.GetProductByIdAsync(id);
                if (productoDto == null)
                    return NotFound();

                var producto = new Producto
                {
                    Id = productoDto.Id,
                    Clave = productoDto.Code,
                    Nombre = productoDto.Name,
                    TipoProducto = productoDto.ProductTypeName ?? "Sin tipo",
                    EsActivo = productoDto.Active,
                    Precio = productoDto.Price ?? 0,
                    Proveedores = new List<Proveedor>()
                };

                ViewBag.TiposProducto = await ObtenerTiposProducto();
                return View(producto);
            }
            catch (InvalidProductException ex)
            {
                _logger.LogWarning($"Producto no encontrado: {ex.Message}");
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener producto: {ex.Message}");
                return StatusCode(500, "Error al obtener el producto");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Producto producto)
        {
            try
            {
                if (id == 0)
                {
                    // Crear nuevo producto
                    var createDto = new CreateProductDto
                    {
                        Code = producto.Clave ?? "",
                        Name = producto.Nombre ?? "",
                        ProductTypeId = 1, // Por defecto, se debe enviar desde la vista
                        Price = producto.Precio
                    };

                    await _productService.CreateProductAsync(createDto);
                }
                else
                {
                    // Actualizar producto existente
                    var updateDto = new UpdateProductDto
                    {
                        Id = id,
                        Code = producto.Clave ?? "",
                        Name = producto.Nombre ?? "",
                        ProductTypeId = 1, // Por defecto, se debe enviar desde la vista
                        Price = producto.Precio
                    };

                    await _productService.UpdateProductAsync(updateDto);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (ProductTypeNotFoundException ex)
            {
                _logger.LogWarning($"Tipo de producto no encontrado: {ex.Message}");
                ModelState.AddModelError("", "El tipo de producto especificado no existe");
                ViewBag.TiposProducto = await ObtenerTiposProducto();
                return View(producto);
            }
            catch (InvalidProductException ex)
            {
                _logger.LogWarning($"Error de validación: {ex.Message}");
                ModelState.AddModelError("", ex.Message);
                ViewBag.TiposProducto = await ObtenerTiposProducto();
                return View(producto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al guardar producto: {ex.Message}");
                ModelState.AddModelError("", "Error al guardar el producto");
                ViewBag.TiposProducto = await ObtenerTiposProducto();
                return View(producto);
            }
        }

        public IActionResult Create()
        {
            var nuevoProducto = new Producto { Id = 0 };
            return RedirectToAction(nameof(Edit), new { id = 0 });
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _productService.DeleteProductAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar producto: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// Obtiene lista de tipos de producto para la vista
        /// </summary>
        private async Task<List<ProductTypeDto>> ObtenerTiposProducto()
        {
            try
            {
                var tipos = await _productTypeService.GetAllProductTypesAsync();
                return tipos.ToList();
            }
            catch
            {
                return new List<ProductTypeDto>();
            }
        }
    }
}
