using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Verasoft.EF
{
    public interface ITrackingEntityActionSubscriber<TDbContext, TEntity, TTrackingItem> 
        where TDbContext : DbContext
        where TEntity : class
        where TTrackingItem : TrackingEntityItem<TEntity>
    {
        void Perform(Action<TDbContext, TTrackingItem> action);
    }
}
