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
        private readonly ProductSupplierApplicationService _productSupplierService;
        private readonly SupplierApplicationService _supplierService;

        public HomeController(
            ILogger<HomeController> logger,
            ProductApplicationService productService,
            ProductTypeApplicationService productTypeService,
            ProductSupplierApplicationService productSupplierService,
            SupplierApplicationService supplierService)
        {
            _logger = logger;
            _productService = productService;
            _productTypeService = productTypeService;
            _productSupplierService = productSupplierService;
            _supplierService = supplierService;
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
                var productos = productosFiltrados.Select(p => new Producto
                {
                    Id = p.Id,
                    Clave = p.Code,
                    Nombre = p.Name,
                    ProductTypeId = p.ProductTypeId,
                    TipoProducto = p.ProductTypeName ?? "Sin tipo",
                    EsActivo = p.Active,
                    Precio = p.Price ?? 0,
                    Proveedores = new List<Proveedor>()
                }).ToList();

                // Pasar tipos de producto a la vista para el dropdown
                ViewBag.TiposProducto = await ObtenerTiposProducto();

                return View(productos);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener productos: {ex.Message}");
                ViewBag.TiposProducto = new List<ProductTypeDto>();
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
                    ProductTypeId = productoDto.ProductTypeId,
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
                if (producto.ProductTypeId <= 0)
                {
                    ModelState.AddModelError("productTypeId", "Debe seleccionar un tipo de producto");
                    ViewBag.TiposProducto = await ObtenerTiposProducto();
                    return View(producto);
                }

                if (id == 0)
                {
                    // Crear nuevo producto
                    var createDto = new CreateProductDto
                    {
                        Code = producto.Clave ?? "",
                        Name = producto.Nombre ?? "",
                        ProductTypeId = producto.ProductTypeId,
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
                        ProductTypeId = producto.ProductTypeId,
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

        // ===== ACCIONES AJAX PARA PROVEEDORES =====

        /// <summary>
        /// Obtiene todos los proveedores disponibles para el formulario
        /// </summary>
        [HttpGet]
        [Route("api/suppliers")]
        public async Task<IActionResult> GetSuppliers()
        {
            try
            {
                var suppliers = await _supplierService.GetAllSuppliersAsync();
                return Json(suppliers.Select(s => new { id = s.Id, name = s.Name }).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener proveedores: {ex.Message}");
                return StatusCode(500, new { error = "Error al obtener proveedores" });
            }
        }

        /// <summary>
        /// Obtiene los proveedores asociados a un producto
        /// </summary>
        [HttpGet]
        [Route("api/product-suppliers/{productId}")]
        public async Task<IActionResult> GetProductSuppliers(int productId)
        {
            try
            {
                if (productId <= 0)
                    return BadRequest(new { error = "ID de producto inválido" });

                var suppliers = await _productSupplierService.GetSuppliersByProductIdAsync(productId);
                return Json(suppliers);
            }
            catch (InvalidProductException ex)
            {
                _logger.LogWarning($"Producto no encontrado: {ex.Message}");
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener proveedores del producto: {ex.Message}");
                return StatusCode(500, new { error = "Error al obtener proveedores" });
            }
        }

        /// <summary>
        /// Obtiene un proveedor específico por ID (para cargar datos en el modal de edición)
        /// </summary>
        [HttpGet]
        [Route("api/product-supplier/{id}")]
        public async Task<IActionResult> GetProductSupplier(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { error = "ID inválido" });

                var supplier = await _productSupplierService.GetProductSupplierByIdAsync(id);
                if (supplier == null)
                    return NotFound(new { error = "Relación no encontrada" });

                return Json(supplier);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener proveedor: {ex.Message}");
                return StatusCode(500, new { error = "Error al obtener proveedor" });
            }
        }

        /// <summary>
        /// Crea o actualiza un proveedor de un producto
        /// </summary>
        [HttpPost]
        [Route("api/product-supplier/save")]
        public async Task<IActionResult> SaveProductSupplier([FromBody] SaveProductSupplierRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest(new { error = "Datos inválidos" });

                if (request.ProductId <= 0 || request.SupplierId <= 0)
                    return BadRequest(new { error = "ProductId y SupplierId son requeridos" });

                ProductSupplierDto result;

                if (request.Id <= 0)
                {
                    // Crear nuevo
                    var createDto = new CreateProductSupplierDto
                    {
                        ProductId = request.ProductId,
                        SupplierId = request.SupplierId,
                        SupplierProductCode = request.SupplierProductCode,
                        Cost = request.Cost
                    };
                    result = await _productSupplierService.CreateProductSupplierAsync(createDto);
                }
                else
                {
                    // Actualizar existente
                    var updateDto = new UpdateProductSupplierDto
                    {
                        Id = request.Id,
                        ProductId = request.ProductId,
                        SupplierId = request.SupplierId,
                        SupplierProductCode = request.SupplierProductCode,
                        Cost = request.Cost
                    };
                    result = await _productSupplierService.UpdateProductSupplierAsync(updateDto);
                }

                return Json(new { success = true, data = result });
            }
            catch (InvalidProductException ex)
            {
                _logger.LogWarning($"Error de validación: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al guardar proveedor: {ex.Message}");
                return StatusCode(500, new { error = "Error al guardar proveedor" });
            }
        }

        /// <summary>
        /// Elimina un proveedor de un producto
        /// </summary>
        [HttpPost]
        [Route("api/product-supplier/{id}/delete")]
        public async Task<IActionResult> DeleteProductSupplier(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { error = "ID inválido" });

                var result = await _productSupplierService.DeleteProductSupplierAsync(id);
                if (!result)
                    return NotFound(new { error = "Relación no encontrada" });

                return Json(new { success = true });
            }
            catch (InvalidProductException ex)
            {
                _logger.LogWarning($"Error: {ex.Message}");
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar proveedor: {ex.Message}");
                return StatusCode(500, new { error = "Error al eliminar proveedor" });
            }
        }
    }
}
