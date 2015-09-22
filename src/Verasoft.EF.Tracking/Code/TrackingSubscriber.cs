using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Verasoft.EF
{
    public class TrackingSubscriber<TDbContext> : ITrackingSubscriber<TDbContext> where TDbContext : DbContext
    {
        #region Fields

        private TrackingEvent @event = TrackingEvent.AfterSaveChanges;
        private IList<ITrackingSubscription> subscriptions = new List<ITrackingSubscription>();

        #endregion

        #region Properties

        internal IList<ITrackingSubscription> Subscriptions
        {
            get { return subscriptions; }
        }

        #endregion

        #region Constructor

        public TrackingSubscriber(TrackingEvent @event)
        {
            this.@event = @event;
        }

        #endregion

        #region ITrackingSubscriber

        public ITrackingEntitySubscriber<TDbContext, TEntity> For<TEntity>()
            where TEntity : class
        {
            return new TrackingEntitySubscriber<TDbContext, TEntity>(new TrackingSubscription<TDbContext, TEntity>(this) { Event = this.@event });
        }

        public void Perform(Action<TDbContext> action)
        {
            var newSubscription = new TrackingSubscription<TDbContext>(this) { Event = this.@event };
            newSubscription.Action = action;
            this.Subscriptions.Add(newSubscription);
        }

        #endregion
    }
}
