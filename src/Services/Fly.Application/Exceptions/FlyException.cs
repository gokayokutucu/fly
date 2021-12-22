using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Application.Exceptions
{
    public class FlyException : Exception
    {
        public FlyException(string name, string message, Exception ex)
            : base($"Service of application \"{name}\" is failed. {message}", ex)
        {
        }
    }
}
