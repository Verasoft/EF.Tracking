using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Verasoft.EF
{
    public interface ITrackingEntitySubscriber<TDbContext, TEntity>
        where TDbContext : DbContext
        where TEntity : class
    {
        ITrackingEntityActionSubscriber<TDbContext,TEntity, TrackingEntityItem<TEntity>> Where(EntityTrackingTypes trackingType);
        ITrackingEntityPropertyChangedActionSubscriber<TDbContext, TEntity> Where(Expression<Func<TEntity, object>> property);
        ITrackingEntityRelationshipActionSubscriber<TDbContext, TEntity, TRelatedEntity> Where<TRelatedEntity>(RelatedEntityTrackingTypes trackingType)
            where TRelatedEntity : class;
        ITrackingEntityRelationshipActionSubscriber<TDbContext, TEntity, TRelatedEntity> Where<TRelatedEntity>(Expression<Func<TEntity, System.Collections.Generic.ICollection<TRelatedEntity>>> relationship, RelatedEntityTrackingTypes trackingType)
            where TRelatedEntity : class;
    }
}
