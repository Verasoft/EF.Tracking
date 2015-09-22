using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Verasoft.EF
{
    public class TrackingEntitySubscriber<TDbContext, TEntity> : ITrackingEntitySubscriber<TDbContext, TEntity>
        where TDbContext : DbContext
        where TEntity : class
    {
        #region Fields

        private TrackingSubscription<TDbContext, TEntity> subscription = null;

        #endregion

        #region Constructor

        public TrackingEntitySubscriber(TrackingSubscription<TDbContext> subscription)
        {
            this.subscription = new TrackingSubscription<TDbContext, TEntity>(subscription.Subscriber, subscription);
        }

        #endregion

        #region ITrackingEntitySubscriber

        public ITrackingEntityActionSubscriber<TDbContext, TEntity, TrackingEntityItem<TEntity>> Where(EntityTrackingTypes trackingType)
        {
            this.subscription.TrackingType = trackingType;
            return new TrackingEntityActionSubscriber<TDbContext, TEntity, TrackingEntityItem<TEntity>>(this.subscription);
        }

        public ITrackingEntityPropertyChangedActionSubscriber<TDbContext, TEntity> Where(Expression<Func<TEntity, object>> property)
        {
            this.subscription.TrackingType = EntityTrackingTypes.Modified;
            this.subscription.Property = property;
            return new TrackingEntityPropertyChangedActionSubscriber<TDbContext, TEntity>(this.subscription);
        }

        public ITrackingEntityRelationshipActionSubscriber<TDbContext, TEntity, TRelatedEntity> Where<TRelatedEntity>(RelatedEntityTrackingTypes trackingType) where TRelatedEntity : class
        {
            var newSubscription = new TrackingSubscription<TDbContext, TEntity, TRelatedEntity>(this.subscription.Subscriber, this.subscription);
            newSubscription.TrackingType = EntityTrackingTypes.Modified;
            newSubscription.RelatedEntityTrackingType = trackingType;
            return new TrackingEntityRelationshipActionSubscriber<TDbContext, TEntity, TRelatedEntity>(newSubscription);
        }

        public ITrackingEntityRelationshipActionSubscriber<TDbContext, TEntity, TRelatedEntity> Where<TRelatedEntity>(Expression<Func<TEntity, System.Collections.Generic.ICollection<TRelatedEntity>>> relatedEntitySet, RelatedEntityTrackingTypes trackingType)
            where TRelatedEntity : class
        {
            var newSubscription = new TrackingSubscription<TDbContext, TEntity, TRelatedEntity>(this.subscription.Subscriber, this.subscription);
            newSubscription.TrackingType = EntityTrackingTypes.Modified;
            newSubscription.RelatedEntitySet = relatedEntitySet;
            newSubscription.RelatedEntityTrackingType = trackingType;
            return new TrackingEntityRelationshipActionSubscriber<TDbContext, TEntity, TRelatedEntity>(newSubscription);
        }

        #endregion
    }
}
