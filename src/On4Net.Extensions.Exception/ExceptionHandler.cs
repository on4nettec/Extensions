using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;   



namespace On4Net.Extensions.Exception;


public static class ExceptionHandler
{
    

    public static ErrorResponse GetErrorFromException(
     this   System.Exception exception
    )
    {
        var result = new ErrorResponse();
        AppException appException= exception as AppException;
        if (appException != null)
        {
            result.MessageId = appException.MessageId;
            result.Message = appException.Message;
            result.StatusCode = appException.StatusCode;
            result.Params = new Dictionary<string, object> { { "Params", appException.Params ?? Array.Empty<object>() } };
        }
        
        switch (exception)
        {
            case NotFoundException notFound:
                result.Params = new Dictionary<string, object> { { "EntityName", notFound.EntityName } };
                break;
            case System.ComponentModel.DataAnnotations.ValidationException validation:
                result.Params = validation.Data.OfType<KeyValuePair<string, object>>()
                                .ToDictionary(k => k.Key.ToString(), v => v.Value!);
                break;
            case DuplicateKeyException duplicateKey:
                result.Params = new Dictionary<string, object> { { "EntityName", duplicateKey.EntityName } };
                break;
            case On4Net.Extensions.Exception.ArgumentException argument:
                result.Params = argument.Data.OfType<KeyValuePair<string, object>>()
                                .ToDictionary(k => k.Key.ToString(), v => v.Value!);
                break;
            
            case NotImplementedException or NotSupportedException:
                result.MessageId = "NotImplementedOrNotSupported";
                result.Message = exception.Message;
                result.StatusCode = 500;
                break;
            case ArgumentNullException or ArgumentOutOfRangeException or System.ArgumentException:
                result.MessageId = exception.Message;
                result.Message = exception.Message;
                result.StatusCode = 400;
                break;
            default:
                result.MessageId = "Exception";
                result.Message = exception.Message;
                result.StatusCode = 500;
                break;
        }
        
        return result;
    }
}
