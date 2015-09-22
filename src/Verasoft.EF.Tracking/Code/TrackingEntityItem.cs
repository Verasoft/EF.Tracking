using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Verasoft.EF
{
    public class TrackingEntityItem<TEntity> where TEntity : class
    {
        public TEntity Entity { get; internal set; }
    }
}
