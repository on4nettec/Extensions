using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace On4Net.Extensions.Exception;

public class NotFoundException : AppException
{
    public string EntityName { get; }

    public NotFoundException(string entityName, int statusCode = 404, object[]? @params = null)
        : base("NotFound", "A `" + entityName + "` was not found.", statusCode, @params)
    {
        EntityName = entityName;
    }

    public NotFoundException(string entityName, object id, int statusCode = 404, object[]? @params = null)
        : base("NotFound", $"A `{entityName}` with id of `{id}` was not found.", statusCode, @params)
    {
        EntityName = entityName;
    }
    public NotFoundException(string entityName, string message, int statusCode = 404, object[]? @params = null)
       : base("NotFound", message, statusCode, @params)
    {
        EntityName = entityName;
    }
}