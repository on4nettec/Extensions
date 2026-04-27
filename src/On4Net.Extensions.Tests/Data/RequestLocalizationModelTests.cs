using System.Linq;
using On4Net.Extensions.Data.Model.Request;
using Xunit;

namespace On4Net.Extensions.Tests.Data;

/// <summary>
/// Property and assignment smoke tests for localization request DTOs under <c>Model.Request</c>.
/// </summary>
public sealed class RequestLocalizationModelTests
{
    private sealed class PlainLocalization : BaseLocalization { }

    private sealed class PlainLocalizationRequest : BaseLocalizationRequest<PlainLocalization> { }

    [Fact]
    public void BaseTitleLocalization_holds_id_culture_version_and_title()
    {
        var id = Guid.NewGuid();
        var row = new BaseTitleLocalization
        {
            Id = id,
            Culture = "fa-IR",
            Version = 2,
            Title = "عنوان"
        };
        Assert.Equal(id, row.Id);
        Assert.Equal("fa-IR", row.Culture);
        Assert.Equal(2, row.Version);
        Assert.Equal("عنوان", row.Title);
    }

    [Fact]
    public void BaseTitleLocalizationRequest_exposes_localization_sequence()
    {
        var req = new BaseTitleLocalizationRequest
        {
            Localization = new[]
            {
                new BaseTitleLocalization { Culture = "en-US", Title = "A" },
                new BaseTitleLocalization { Culture = "fa-IR", Title = "ب" }
            }
        };
        var items = req.Localization!.ToList();
        Assert.Equal(2, items.Count);
        Assert.Equal("en-US", items[0].Culture);
        Assert.Equal("ب", items[1].Title);
    }

    [Fact]
    public void BaseTitleLocalizationRequest_can_be_used_as_BaseLocalizationRequest_of_title_row()
    {
        BaseLocalizationRequest<BaseTitleLocalization> req = new BaseTitleLocalizationRequest
        {
            Localization = Array.Empty<BaseTitleLocalization>()
        };
        Assert.Empty(req.Localization!);
    }

    [Fact]
    public void BaseLocalizationRequest_generic_accepts_custom_T_derived_from_BaseLocalization()
    {
        var req = new PlainLocalizationRequest
        {
            Localization = new[] { new PlainLocalization { Culture = "de-DE", Id = Guid.NewGuid() } }
        };
        var first = req.Localization!.First();
        Assert.Equal("de-DE", first.Culture);
    }
}
