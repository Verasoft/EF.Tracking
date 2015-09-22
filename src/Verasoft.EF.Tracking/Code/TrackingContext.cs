using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Verasoft.EF
{
    public class TrackingContext : ITrackingContext
    {
        #region Fields

        protected List<ITrackingSubscription> subscriptions = new List<ITrackingSubscription>();
        protected IList<Guid> tracking = new List<Guid>();

        #endregion

        #region Properties

        public static TrackingContext Current
        {
            get { return TrackingContextFactory.Current; }
        }

        #endregion

        internal virtual void BeforeDbContextSaveChanges(ITrackingDbContext context, TrackingSaveChangeOptions options = null)
        {
            if (!tracking.Contains(context.ID))
            {
                lock (this)
                {
                    tracking.Add(context.ID);
                }

                var validSubscriptions = this.subscriptions.Where(x => x.IsForDbContext(context as DbContext) && !x.IsIgnored(options));
                foreach (var subscription in validSubscriptions)
                {
                    try
                    {
                        subscription.Verify(context as DbContext);
                    }
                    catch (Exception ex)
                    {

                    }
                }

                foreach (var subscription in validSubscriptions.Where(x => x.Event == TrackingEvent.BeforeSaveChanges && x.Handle && !x.Running))
                {
                    subscription.Perform(context as DbContext);
                }

                lock (this)
                {
                    tracking.Remove(context.ID);
                }
            }
        }

        internal virtual void AfterDbContextSaveChanges(ITrackingDbContext context, CancellationToken cancellationToken, TrackingSaveChangeOptions options = null)
        {
            if (!tracking.Contains(context.ID))
            {
                lock (this)
                {
                    tracking.Add(context.ID);
                }

                var validSubscriptions = this.subscriptions.Where(x => x.IsForDbContext(context as DbContext) && !x.IsIgnored(options));
                foreach (var subscription in validSubscriptions.Where(x => x.Event == TrackingEvent.AfterSaveChanges && x.Handle && !x.Running))
                {
                    try
                    {
                        subscription.Perform(context as DbContext);
                    }
                    catch (Exception ex)
                    {

                    }
                }

                lock (this)
                {
                    tracking.Remove(context.ID);
                }
            }
        }

        internal virtual void BeforeDbModelCreation(ITrackingDbContext context, DbModelBuilder modelBuilder)
        {

        }

        internal virtual void AfterDbModelCreation(ITrackingDbContext context, DbModelBuilder modelBuilder)
        {

        }

        public virtual void Subscribe(TrackingEvent eventType, Action<ITrackingSubscriber<DbContext>> subscriber)
        {
            var s = new TrackingSubscriber<DbContext>(eventType);
            subscriber(s);
            this.subscriptions.AddRange(s.Subscriptions);
        }

        public virtual void Subscribe<TDbContext>(TrackingEvent eventType, Action<ITrackingSubscriber<TDbContext>> subscriber) where TDbContext : DbContext
        {
            var s = new TrackingSubscriber<TDbContext>(eventType);
            subscriber(s);
            this.subscriptions.AddRange(s.Subscriptions);
        }

        public virtual void ClearSubscriptions()
        {
            subscriptions.Clear();
        }
    }
}
