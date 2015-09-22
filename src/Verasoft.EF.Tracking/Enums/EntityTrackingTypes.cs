using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Verasoft.EF
{
    [Flags]
    public enum EntityTrackingTypes
    {
        Added = 2,
        Modified = 4,
        Deleted = 8
    }
}
