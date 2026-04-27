namespace On4Net.Extensions.Data.Model.Request;

public abstract class BaseLocalization
{
    public Guid Id { get; set; }
    public string Culture { get; set; }

    public int? Version { get; set; }
}
