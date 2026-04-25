using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace On4Net.Extensions.Data.DataManager;

public class PagedResult<T> where T : class
{
    public List<T> Items { get; set; } = new List<T>();


    public int TotalCount { get; set; }
}