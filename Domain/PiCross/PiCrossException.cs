using System;

namespace PiCross
{
    /// <summary>
    ///     Base class for all PiCross-related exceptions.
    /// </summary>
    public abstract class PiCrossException : Exception
    {
        public PiCrossException(string message)
            : base(message)
        {
            // NOP
        }

        public PiCrossException(string message, Exception innerException)
            : base(message, innerException)
        {
            // NOP
        }
    }
}