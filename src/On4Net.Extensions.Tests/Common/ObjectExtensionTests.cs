using System.Data;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using On4Net.Extensions.Common;
using Xunit;

namespace On4Net.Extensions.Tests.Common;

/// <summary>
/// Tests for <see cref="ObjectExtension"/> compression, JSON, binding, and factory helpers.
/// </summary>
public sealed class ObjectExtensionTests
{
    [Fact]
    public void Compress_then_Decompress_roundtrips_bytes()
    {
        var original = Encoding.UTF8.GetBytes("on4net-extensions");
        var compressed = original.Compress();
        Assert.NotEqual(original, compressed);
        var restored = compressed.Decompress();
        Assert.Equal(original, restored);
    }

    [Fact]
    public void DecompressMemoryStream_returns_stream_that_is_disposed_after_method_returns()
    {
        var original = Encoding.UTF8.GetBytes("payload");
        var compressed = original.Compress();
        var stream = compressed.DecompressMemoryStream();
        Assert.Throws<ObjectDisposedException>(() => stream.ReadByte());
    }

    [Fact]
    public void GetNumberByGuid_Guid_is_deterministic_for_same_guid()
    {
        var g = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");
        var n1 = g.GetNumberByGuid();
        var n2 = g.GetNumberByGuid();
        Assert.Equal(n1, n2);
        Assert.False(string.IsNullOrEmpty(n1));
    }

    [Fact]
    public void GetNumberByGuid_string_parses_and_matches_guid_form()
    {
        var g = Guid.NewGuid();
        var fromString = g.ToString().GetNumberByGuid();
        var fromGuid = g.GetNumberByGuid();
        Assert.Equal(fromGuid, fromString);
    }

    private sealed class Row
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }

    [Fact]
    public void ConvertToDatatable_builds_columns_and_rows()
    {
        var list = new List<Row> { new() { Id = 1, Name = "A" }, new() { Id = 2, Name = "B" } };
        var table = list.ConvertToDatatable("rows");
        Assert.Equal("rows", table.TableName);
        Assert.Equal(2, table.Rows.Count);
        Assert.Equal(1, table.Rows[0]["Id"]);
        Assert.Equal("A", table.Rows[0]["Name"]);
    }

    private sealed class SimpleDto
    {
        public int X { get; set; }
        public string Y { get; set; } = "";
    }

    [Fact]
    public void BindDataClass_copies_matching_properties()
    {
        var dest = new SimpleDto { X = 0, Y = "old" };
        var src = new SimpleDto { X = 42, Y = "new" };
        dest.BindDataClass(src);
        Assert.Equal(42, dest.X);
        Assert.Equal("new", dest.Y);
    }

    [Fact]
    public void ToDateTime_converts_unix_seconds_to_local()
    {
        var dt = 0L.ToDateTime();
        Assert.Equal(DateTimeKind.Local, dt.Kind);
    }

    [Fact]
    public void GetTime_returns_positive_epoch_seconds()
    {
        var t = ObjectExtension.GetTime();
        Assert.True(t > 1_000_000_000);
    }

    private enum SampleEnum
    {
        Alpha,
        Beta
    }

    [Fact]
    public void ToEnum_parses_defined_member()
    {
        var e = "Beta".ToEnum<SampleEnum>();
        Assert.Equal(SampleEnum.Beta, e);
    }

    [Fact]
    public void ToEnum_invalid_throws_ArgumentException()
    {
        Assert.Throws<ArgumentException>(() => "Gamma".ToEnum<SampleEnum>());
    }

    [Fact]
    public void ToJsonString_serializes_object()
    {
        var json = new { a = 1 }.ToJsonString();
        Assert.Contains("\"a\"", json);
    }

    [Fact]
    public void ToJsonString_null_object_throws_ArgumentNullException()
    {
        object? o = null;
        Assert.Throws<ArgumentNullException>(() => o!.ToJsonString());
    }

    [Fact]
    public void FromJsonString_deserializes()
    {
        var obj = "{\"x\":3}".FromJsonString<Dictionary<string, int>>();
        Assert.Equal(3, obj["x"]);
    }

    [Fact]
    public void FromJsonString_whitespace_throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => "   ".FromJsonString<object>());
    }

    [Fact]
    public void CreateInstance_generic_creates_new_instance()
    {
        object? receiver = null;
        var instance = receiver.CreateInstance<object>();
        Assert.NotNull(instance);
    }
}
