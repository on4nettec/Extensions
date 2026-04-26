namespace On4Net.Extensions.Exception;

/// <summary>
/// General-purpose application error for unforeseen or unexpected situations, with the same constructor contract as <see cref="AppException"/>.
/// </summary>
/// <remarks>
/// Full type name is <c>On4Net.Extensions.Exception.ApplicationException</c> to distinguish it from <see cref="System.ApplicationException"/>.
/// </remarks>
public class ApplicationException : AppException
{
    public ApplicationException(string messageId, string message, int statusCode = 500, object[]? @params = null)
        : base(messageId, message, statusCode, @params)
    {
    }
}
