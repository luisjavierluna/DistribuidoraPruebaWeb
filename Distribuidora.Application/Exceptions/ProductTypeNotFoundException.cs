using System;

namespace Distribuidora.Application.Exceptions
{
    /// <summary>
    /// Se lanza cuando no se encuentra un tipo de producto
    /// </summary>
    public class ProductTypeNotFoundException : ApplicationException
    {
        public ProductTypeNotFoundException(int id) 
            : base($"Product Type with id '{id}' was not found.") { }
    }
}
