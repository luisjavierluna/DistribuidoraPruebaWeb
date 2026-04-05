using System;

namespace Distribuidora.Application.Exceptions
{
    /// <summary>
    /// Se lanza cuando un producto es inválido según las reglas de negocio
    /// </summary>
    public class InvalidProductException : ApplicationException
    {
        public InvalidProductException(string message) 
            : base($"Invalid Product: {message}") { }
    }
}
