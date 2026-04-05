using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebAppMVC.Models;

namespace WebAppMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        // Datos hardcodeados
        private static List<Producto> productos = new List<Producto>
        {
            new Producto
            {
                Id = 1,
                Clave = "PIN0259",
                Nombre = "Pinol Limpiador 360 ml",
                TipoProducto = "Limpieza",
                EsActivo = true,
                Precio = 20.50m,
                Proveedores = new List<Proveedor>
                {
                    new Proveedor { Id = 1, Nombre = "Distribuidor Mexico", ClaveProducto = "PINOL360", Costo = 20.50m },
                    new Proveedor { Id = 2, Nombre = "Abarrottes a Granel Ruiz", ClaveProducto = "P123450", Costo = 21.00m },
                    new Proveedor { Id = 3, Nombre = "Surtidra La Morena", ClaveProducto = "LIMP-PINOL01", Costo = 19.80m }
                }
            },
            new Producto
            {
                Id = 2,
                Clave = "PIN0152",
                Nombre = "Pinol Limpiador 250 ml",
                TipoProducto = "Limpieza",
                EsActivo = true,
                Precio = 15.75m,
                Proveedores = new List<Proveedor>
                {
                    new Proveedor { Id = 4, Nombre = "Distribuidor Mexico", ClaveProducto = "PINOL250", Costo = 15.75m }
                }
            },
            new Producto
            {
                Id = 3,
                Clave = "JBC001",
                Nombre = "Jabón en barra",
                TipoProducto = "Higiene",
                EsActivo = true,
                Precio = 5.50m,
                Proveedores = new List<Proveedor>
                {
                    new Proveedor { Id = 5, Nombre = "Abarrottes a Granel Ruiz", ClaveProducto = "JAB001", Costo = 5.50m }
                }
            }
        };

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(string clave = "", string tipoProducto = "")
        {
            var productosFiltrados = productos;

            if (!string.IsNullOrEmpty(clave))
            {
                productosFiltrados = productosFiltrados.Where(p => p.Clave != null && p.Clave.Contains(clave, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(tipoProducto))
            {
                productosFiltrados = productosFiltrados.Where(p => p.TipoProducto != null && p.TipoProducto.Contains(tipoProducto, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return View(productosFiltrados);
        }

        public IActionResult Edit(int id)
        {
            var producto = productos.FirstOrDefault(p => p.Id == id);
            if (producto == null)
            {
                return NotFound();
            }
            return View(producto);
        }

        [HttpPost]
        public IActionResult Edit(int id, Producto producto)
        {
            var productoExistente = productos.FirstOrDefault(p => p.Id == id);
            if (productoExistente == null)
            {
                return NotFound();
            }

            productoExistente.Clave = producto.Clave;
            productoExistente.Nombre = producto.Nombre;
            productoExistente.TipoProducto = producto.TipoProducto;
            productoExistente.EsActivo = producto.EsActivo;

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Create()
        {
            var nuevoProducto = new Producto { Id = 0 };
            return View("Edit", nuevoProducto);
        }

        [HttpPost]
        public IActionResult Create(Producto producto)
        {
            if (string.IsNullOrEmpty(producto.Clave) || string.IsNullOrEmpty(producto.Nombre) || string.IsNullOrEmpty(producto.TipoProducto))
            {
                return View("Edit", producto);
            }

            // Generar nuevo ID
            int nuevoId = productos.Max(p => p.Id) + 1;
            producto.Id = nuevoId;
            producto.Proveedores = new List<Proveedor>();

            productos.Add(producto);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var producto = productos.FirstOrDefault(p => p.Id == id);
            if (producto != null)
            {
                productos.Remove(producto);
            }
            return RedirectToAction(nameof(Index));
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
    }
}
