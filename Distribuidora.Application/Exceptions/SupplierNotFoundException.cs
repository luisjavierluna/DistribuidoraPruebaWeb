using System;

namespace Distribuidora.Application.Exceptions
{
    /// <summary>
    /// Se lanza cuando no se encuentra un proveedor
    /// </summary>
    public class SupplierNotFoundException : ApplicationException
    {
        public SupplierNotFoundException(int id) 
            : base($"Supplier with id '{id}' was not found.") { }
    }
}
