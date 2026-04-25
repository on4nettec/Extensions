
namespace On4Net.Extensions.Exception;

public class ErrorResponse
{
    public int StatusCode { get; set; }

    public string MessageId { get; set; }

    public string Message { get; set; }

    public Dictionary<string, object> Params { get; set; }
}