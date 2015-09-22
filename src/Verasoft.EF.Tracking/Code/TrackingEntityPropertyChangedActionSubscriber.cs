using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Verasoft.EF
{
    public class TrackingEntityPropertyChangedActionSubscriber<TDbContext, TEntity> : ITrackingEntityPropertyChangedActionSubscriber<TDbContext, TEntity>
        where TDbContext : DbContext
        where TEntity : class
    {
        #region Fields

        private TrackingSubscription<TDbContext, TEntity> subscription = null;

        #endregion

        #region Constructor

        public TrackingEntityPropertyChangedActionSubscriber(TrackingSubscription<TDbContext, TEntity> subscription)
        {
            this.subscription = subscription;
        }

        #endregion

        #region ITrackingTypeSubscriber

        public ITrackingEntityActionSubscriber<TDbContext, TEntity, TrackingEntityPropertyChangedItem<TEntity>> To(object value)
        {
            this.subscription.PropertyValue = value;
            return new TrackingEntityPropertyChangedActionSubscriber<TDbContext, TEntity>(this.subscription);
        }

        public void Perform(Action<TDbContext, TrackingEntityPropertyChangedItem<TEntity>> action)
        {
            this.subscription.Action = action;
            this.subscription.Subscriber.Subscriptions.Add(this.subscription);
        }

        #endregion
    }
}
