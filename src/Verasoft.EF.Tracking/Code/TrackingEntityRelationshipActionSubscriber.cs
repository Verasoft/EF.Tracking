using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Verasoft.EF
{
    public class TrackingEntityRelationshipActionSubscriber<TDbContext, TEntity, TRelatedEntity> : ITrackingEntityRelationshipActionSubscriber<TDbContext, TEntity, TRelatedEntity>
        where TDbContext : DbContext
        where TEntity : class
        where TRelatedEntity : class
    {
        #region Fields

        private TrackingSubscription<TDbContext, TEntity, TRelatedEntity> subscription = null;

        #endregion

        #region Constructor

        public TrackingEntityRelationshipActionSubscriber(TrackingSubscription<TDbContext, TEntity, TRelatedEntity> subscription)
        {
            this.subscription = subscription;
        }

        #endregion

        #region ITrackingEntityRelationshipActionSubscriber

        public void Perform(Action<TDbContext, TrackingEntityRelationshipItem<TEntity, TRelatedEntity>> action)
        {
            this.subscription.Action = action;
            this.subscription.Subscriber.Subscriptions.Add(this.subscription);
        }

        #endregion
    }
}
