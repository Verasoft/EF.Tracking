using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Verasoft.EF
{
    public interface ITrackingSubscription
    {
        TrackingEvent Event { get; }
        Delegate Action { get; }
        bool Handle { get; }
        bool Running { get; }

        void Verify(DbContext context);
        void Perform(DbContext context);
        bool IsForDbContext(DbContext context);
        bool IsIgnored(TrackingSaveChangeOptions options);
    }

    public interface ITrackingSubscription<TEntity> : ITrackingSubscription
        where TEntity : class
    {
        EntityTrackingTypes TrackingType { get; }
        Expression<Func<TEntity, object>> Property { get; }
        object PropertyValue { get; }
    }

    public interface ITrackingSubscription<TEntity, TRelatedEntity> : ITrackingSubscription<TEntity>
        where TEntity : class
        where TRelatedEntity : class
    {
        Expression<Func<TEntity, System.Collections.Generic.ICollection<TRelatedEntity>>> RelatedEntitySet { get; }
        RelatedEntityTrackingTypes RelatedEntityTrackingType { get; }
    }
}
