using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace On4Net.Extensions.Exception;

public class ArgumentException : AppException
{
    public ArgumentException(  string message, int statusCode = 403, object[] @params = null) : base("Argument", message, statusCode, @params)
    {
    }
}
