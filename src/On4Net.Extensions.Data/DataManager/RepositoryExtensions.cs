using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace On4Net.Extensions.Data.DataManager;

public static class RepositoryExtensions
{
    public static string ToAndQuery(
   this List<string> parts
)
    {
        return parts is null || parts.Count == 0
            ? "TRUE"
            : string.Join("AND", parts.Select(c => $"({c})"));
    }
    public static string ToOrQuery(this List<string> parts)
    {
        var reternValue = "(";
        reternValue += parts is null || parts.Count == 0
            ? "TRUE"
            : string.Join("OR", parts.Select(c => $" {c} "));
        reternValue = reternValue + ")";
        return reternValue;
    }
}
