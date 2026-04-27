using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using On4Net.Extensions.Data.Model.Entity;

namespace On4Net.Extensions.Data.Model.Request;

public abstract class BaseLocalizationRequest<T> where T : BaseLocalization, new() 
{
    public IEnumerable<T> Localization { get; set; }
}
