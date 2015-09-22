using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Verasoft.EF
{
    public interface ITrackingEntityRelationshipActionSubscriber<TDbContext, TEntity, TRelatedEntity> : ITrackingEntityActionSubscriber<TDbContext, TEntity, TrackingEntityRelationshipItem<TEntity, TRelatedEntity>> 
        where TDbContext : DbContext
        where TEntity : class
        where TRelatedEntity : class
    {
    }
}
