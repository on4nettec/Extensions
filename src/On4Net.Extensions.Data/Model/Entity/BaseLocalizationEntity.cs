using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace On4Net.Extensions.Data.Model.Entity;

public abstract class BaseLocalizationEntity : BaseStatusEntity
{
    public Guid LocaliztionId { get; set; }

    public string Culture { get; set; }
}
