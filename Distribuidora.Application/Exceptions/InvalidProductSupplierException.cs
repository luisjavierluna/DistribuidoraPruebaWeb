using System;

namespace Distribuidora.Application.Exceptions
{
    /// <summary>
    /// Se lanza cuando un ProductSupplier es inválido según las reglas de negocio
    /// </summary>
    public class InvalidProductSupplierException : ApplicationException
    {
        public InvalidProductSupplierException(string message) 
            : base($"Invalid Product Supplier: {message}") { }
    }
}
