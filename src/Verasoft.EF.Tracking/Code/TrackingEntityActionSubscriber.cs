using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Verasoft.EF
{
    public class TrackingEntityActionSubscriber<TDbContext, TEntity, TTrackingItem> : ITrackingEntityActionSubscriber<TDbContext, TEntity, TTrackingItem>
        where TDbContext : DbContext
        where TEntity : class
        where TTrackingItem : TrackingEntityItem<TEntity>
    {
        #region Fields

        private TrackingSubscription<TDbContext, TEntity> subscription = null;

        #endregion

        #region Constructor

        public TrackingEntityActionSubscriber(TrackingSubscription<TDbContext, TEntity> subscription)
        {
            this.subscription = subscription;
        }

        #endregion

        #region ITrackingTypeSubscriber

        public void Perform(Action<TDbContext, TTrackingItem> action)
        {
            this.subscription.Action = action;
            this.subscription.Subscriber.Subscriptions.Add(this.subscription);
        }

        #endregion
    }
}
