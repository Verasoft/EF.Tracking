using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Verasoft.EF
{
    public class TrackingEntityPropertyChangedItem<TEntity> : TrackingEntityItem<TEntity> where TEntity : class
    {
        public PropertyInfo Property { get; internal set; }
        public object OriginalValue { get; internal set; }
        public object CurrentValue { get; internal set; }
    }
}
