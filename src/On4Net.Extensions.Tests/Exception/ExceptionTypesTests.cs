using Xunit;
using AppArgException = On4Net.Extensions.Exception.ArgumentException;
using AppApplicationException = On4Net.Extensions.Exception.ApplicationException;

namespace On4Net.Extensions.Tests.Exception;

/// <summary>
/// Constructor and property tests for domain exceptions.
/// </summary>
public sealed class ExceptionTypesTests
{
    [Fact]
    public void NotFoundException_sets_entity_name_and_message_id()
    {
        var ex = new On4Net.Extensions.Exception.NotFoundException("Order");
        Assert.Equal("Order", ex.EntityName);
        Assert.Equal("NotFound", ex.MessageId);
        Assert.Equal(404, ex.StatusCode);
    }

    [Fact]
    public void DuplicateKeyException_sets_entity_name()
    {
        var ex = new On4Net.Extensions.Exception.DuplicateKeyException("User");
        Assert.Equal("User", ex.EntityName);
        Assert.Equal("DuplicateKey", ex.MessageId);
    }

    [Fact]
    public void ConcurrencyException_default_message_id()
    {
        var ex = new On4Net.Extensions.Exception.ConcurrencyException();
        Assert.Equal("Concurrency", ex.MessageId);
    }

    [Fact]
    public void DataValidationException_uses_validation_message_id()
    {
        var ex = new On4Net.Extensions.Exception.DataValidationException("bad");
        Assert.Equal("Validation", ex.MessageId);
        Assert.Equal(400, ex.StatusCode);
    }

    [Fact]
    public void AppArgument_exception_type_alias()
    {
        var ex = new AppArgException("nope", 403);
        Assert.Equal("Argument", ex.MessageId);
        Assert.Equal(403, ex.StatusCode);
    }

    [Fact]
    public void ApplicationException_matches_AppException_constructor_contract()
    {
        var ex = new AppApplicationException("App.Error", "Something failed", 422, new object[] { "x" });
        Assert.Equal("App.Error", ex.MessageId);
        Assert.Equal("Something failed", ex.Message);
        Assert.Equal(422, ex.StatusCode);
        Assert.Single(ex.Params!);
        Assert.Equal("x", ex.Params![0]);
    }
}
