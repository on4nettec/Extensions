using On4Net.Extensions.Data.DataManager;
using On4Net.Extensions.Data.Model.Entity;
using On4Net.Extensions.Data.Model.Request;
using On4Net.Extensions.Data.Schema;
using Xunit;

namespace On4Net.Extensions.Tests.Data;

/// <summary>
/// Smoke tests for enums and POCOs used across the data layer.
/// </summary>
public sealed class ModelAndDtoTests
{
    [Fact]
    public void Status_enum_has_expected_numeric_values()
    {
        Assert.Equal(0, (int)Status.Deleted);
        Assert.Equal(1, (int)Status.Active);
    }

    [Fact]
    public void SortDirection_enum_values()
    {
        Assert.Equal(0, (int)SortDirection.ASC);
        Assert.Equal(1, (int)SortDirection.DESC);
    }

    [Fact]
    public void BaseSaerchRequest_default_PageSize_is_10()
    {
        var r = new BaseSaerchRequest();
        Assert.Equal(10, r.PageSize);
        Assert.NotNull(r.Orders);
    }

    [Fact]
    public void PagedResult_defaults_items_and_total()
    {
        var p = new PagedResult<object>();
        Assert.NotNull(p.Items);
        Assert.Equal(0, p.TotalCount);
    }

    [Fact]
    public void Message_row_model_roundtrip_properties()
    {
        var m = new Message
        {
            Id = 1,
            MessageId = Guid.NewGuid(),
            QueueUrl = "q",
            GroupId = "g",
            DeduplicationId = "d",
            Payload = "{}",
            CreatedAt = DateTime.UtcNow
        };
        Assert.Equal("q", m.QueueUrl);
        Assert.NotEqual(Guid.Empty, m.MessageId);
    }
}
