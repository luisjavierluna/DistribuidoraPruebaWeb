using System;

namespace Distribuidora.Application.Exceptions
{
    /// <summary>
    /// Excepción base para todas las excepciones de la capa de aplicación
    /// </summary>
    public class ApplicationException : Exception
    {
        public ApplicationException(string message) : base(message) { }
        public ApplicationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
