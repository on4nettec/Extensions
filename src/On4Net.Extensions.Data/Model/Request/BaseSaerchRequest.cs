using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace On4Net.Extensions.Data.Model.Request;

public enum SortDirection
{
    ASC = 0,
    DESC = 1
}

public class BaseSaerchRequest
{
    public string SearchKey { get; set; }
    public string Culture { get; set; }
    public int PageNo { get; set; }
    public int PageSize { get; set; } = 10;
    public Dictionary<string, SortDirection> Orders { get; set; } = new Dictionary<string, SortDirection>();
}
