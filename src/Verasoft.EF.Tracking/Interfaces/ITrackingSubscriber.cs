using System;
using System.Data.Entity;

namespace Verasoft.EF
{
    public interface ITrackingSubscriber<TDbContext> where TDbContext : DbContext
    {
        ITrackingEntitySubscriber<TDbContext, TEntity> For<TEntity>() where TEntity : class;
        void Perform(Action<TDbContext> action);
    }
}
