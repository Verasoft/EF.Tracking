using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Verasoft.EF
{
    public interface ITrackingEntityPropertyChangedActionSubscriber<TDbContext, TEntity> : ITrackingEntityActionSubscriber<TDbContext, TEntity, TrackingEntityPropertyChangedItem<TEntity>>
        where TDbContext : DbContext
        where TEntity : class
    {
        ITrackingEntityActionSubscriber<TDbContext, TEntity, TrackingEntityPropertyChangedItem<TEntity>> To(object value);
    }
}
