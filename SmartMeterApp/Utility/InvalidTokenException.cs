using System.Runtime.Serialization;

namespace SmartMeterApp.Utility
{
    public class InvalidTokenException : Exception
    {
        public InvalidTokenException()
        {
        }

        public InvalidTokenException(string? message) : base(message)
        {
        }

        public InvalidTokenException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidTokenException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
