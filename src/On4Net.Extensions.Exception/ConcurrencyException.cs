using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace On4Net.Extensions.Exception;

public class ConcurrencyException : AppException
{
    public ConcurrencyException()
        : this("Concurrency")
    {
    }

    public ConcurrencyException(string message, int statusCode = 409, object[]? @params = null)
        : base("Concurrency", message, statusCode, @params)
    {
    }
}
