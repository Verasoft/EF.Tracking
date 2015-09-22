using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Verasoft.EF
{
    public interface ITrackingContext
    {
        void Subscribe(TrackingEvent @event, Action<ITrackingSubscriber<DbContext>> @subcriber);
        void Subscribe<TDbContext>(TrackingEvent @event, Action<ITrackingSubscriber<TDbContext>> @subscriber) where TDbContext : DbContext;
    }
}
