using System;

namespace xNet
{
    internal static class ExceptionHelper
    {
        internal static ArgumentException EmptyString(string paramName)
        {
            return new ArgumentException("The value can not be empty.", paramName);
        }

        internal static ArgumentOutOfRangeException CanNotBeLess<T>(string paramName, T value) where T : struct
        {
            return new ArgumentOutOfRangeException(paramName, string.Format(
                    "The value can not be less than {0}.", value));
        }

        internal static ArgumentOutOfRangeException CanNotBeGreater<T>(string paramName, T value) where T : struct
        {
            return new ArgumentOutOfRangeException(paramName, string.Format(
                    "The value can not be more than {0}.", value));
        } 

        internal static ArgumentException WrongPath(string paramName, Exception innerException = null)
        {
            return new ArgumentException("The path is an empty string, contains only white space, or contains invalid characters.", paramName, innerException);
        }

        internal static ArgumentOutOfRangeException WrongTcpPort(string paramName)
        {
            return new ArgumentOutOfRangeException("port", string.Format(
                "The value can not be less than {0} or {1} longer.", 1, 65535));
        }

        internal static bool ValidateTcpPort(int port)
        {
            if (port < 1 || port > 65535)
            {
                return false;
            }

            return true;
        }
    }
}