using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace On4Net.Extensions.Exception;

public class DuplicateKeyException : AppException
{
    public string EntityName { get; }

    public DuplicateKeyException(string entityName, int statusCode = 409, object[] @params = null)
        : base("DuplicateKey", "DuplicateKey", statusCode, @params)
    {
        EntityName = entityName;
    }

    public DuplicateKeyException(string entityName, object id, int statusCode = 409)
        : base("DuplicateKey", "DuplicateKey", statusCode, new object[2] { entityName, id })
    {
        EntityName = entityName;
    }

    public DuplicateKeyException(string entityName, string message, object[] @params = null, int statusCode = 409)
        : base("DuplicateKey", message, statusCode, @params)
    {
        EntityName = entityName;
    }
}