using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace On4Net.Extensions.Exception;

public class DataValidationException : AppException
{
    public DataValidationException(string message, int statusCode = 400, object[]? @params = null)
        : base("Validation", message, statusCode, @params)
    {
    }
}