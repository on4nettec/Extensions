using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace On4Net.Extensions.Exception;

public abstract class AppException : System.Exception
{
    public int StatusCode { get; set; }

    public string MessageId { get; }

    public object[]? Params { get; }

    protected AppException(string messageId, string message, int statusCode = 500, object[]? @params = null)
        : base(message)
    {
        MessageId = messageId;
        StatusCode = statusCode;
        Params = @params;
    }
}