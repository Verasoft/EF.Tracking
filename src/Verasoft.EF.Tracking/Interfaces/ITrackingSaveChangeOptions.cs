using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Verasoft.EF
{
    public interface ITrackingSaveChangeOptions
    {
        ITrackingSaveChangeOptions IgnoreAll();
        ITrackingSaveChangeOptions IgnoreContext<TDbContext>() where TDbContext : DbContext;
        ITrackingSaveChangeOptions IgnoreEntity<TDbContext, TEntity>(EntityTrackingTypes? trackingType = null) where TDbContext : DbContext;
    }
}
