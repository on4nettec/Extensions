using Xunit;
using On4Net.Extensions.Exception;
using AppArgException = On4Net.Extensions.Exception.ArgumentException;

namespace On4Net.Extensions.Tests.Exception;

/// <summary>
/// Tests for <see cref="On4Net.Extensions.Exception.ExceptionHandler.GetErrorFromException"/>.
/// </summary>
public sealed class ExceptionHandlerTests
{
    [Fact]
    public void NotFoundException_maps_entity_name_params()
    {
        var ex = new On4Net.Extensions.Exception.NotFoundException("Item");
        var err = ex.GetErrorFromException();
        Assert.Equal("NotFound", err.MessageId);
        Assert.Contains("EntityName", err.Params.Keys);
        Assert.Equal("Item", err.Params["EntityName"]);
    }

    [Fact]
    public void DuplicateKeyException_maps_entity_name()
    {
        var ex = new On4Net.Extensions.Exception.DuplicateKeyException("Row");
        var err = ex.GetErrorFromException();
        Assert.Equal("Row", err.Params["EntityName"]);
    }

    [Fact]
    public void AppArgument_exception_maps_exception_Data_via_KeyValuePair_projection()
    {
        var ex = new AppArgException("x", 403);
        // `Exception.Data` entries are not `KeyValuePair<string, object>`; the handler only picks up pairs that match that shape.
        var err = ex.GetErrorFromException();
        Assert.Equal("Argument", err.MessageId);
        Assert.NotNull(err.Params);
        Assert.Empty(err.Params);
    }

    [Fact]
    public void NotImplementedException_sets_fixed_message_id()
    {
        var ex = new NotImplementedException("todo");
        var err = ex.GetErrorFromException();
        Assert.Equal("NotImplementedOrNotSupported", err.MessageId);
        Assert.Equal(500, err.StatusCode);
    }

    [Fact]
    public void System_ArgumentException_maps_to_400()
    {
        var ex = new System.ArgumentException("bad arg");
        var err = ex.GetErrorFromException();
        Assert.Equal(400, err.StatusCode);
    }

    [Fact]
    public void Generic_exception_falls_back_to_Exception_message_id()
    {
        var ex = new InvalidOperationException("boom");
        var err = ex.GetErrorFromException();
        Assert.Equal("Exception", err.MessageId);
        Assert.Equal(500, err.StatusCode);
        Assert.Equal("boom", err.Message);
    }

    /// <summary>
    /// Documents current behaviour: <see cref="On4Net.Extensions.Exception.ConcurrencyException"/> is an
    /// <see cref="On4Net.Extensions.Exception.AppException"/> but has no dedicated switch arm, so the default branch runs.
    /// </summary>
    [Fact]
    public void ConcurrencyException_currently_hits_default_switch_branch()
    {
        var ex = new On4Net.Extensions.Exception.ConcurrencyException("lost");
        var err = ex.GetErrorFromException();
        Assert.Equal("Exception", err.MessageId);
        Assert.Equal(500, err.StatusCode);
    }
}
