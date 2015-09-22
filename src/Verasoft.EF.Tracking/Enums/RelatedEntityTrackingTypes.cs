using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Verasoft.EF
{
    [Flags]
    public enum RelatedEntityTrackingTypes
    {
        Added = 2,
        Deleted = 4
    }
}
