using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Verasoft.EF
{
    internal class EntityTrackingTypeIgnoreMap
    {
        public Type DbContext { get; set; }
        public Type Entity { get; set; }
        public EntityTrackingTypes? TrackingType { get; set; }
    }

    public class TrackingSaveChangeOptions : ITrackingSaveChangeOptions
    {
        #region Fields

        internal bool _ignoreAll { get; set; }
        internal IList<Type> _ignoreDbContext { get; set; }
        internal IList<EntityTrackingTypeIgnoreMap> _ignoreEntity { get; set; }

        #endregion

        #region Properties



        #endregion

        #region Constructor

        public TrackingSaveChangeOptions()
        {
            _ignoreDbContext = new List<Type>();
            _ignoreEntity = new List<EntityTrackingTypeIgnoreMap>();
        }

        #endregion


        public ITrackingSaveChangeOptions IgnoreAll()
        {
            _ignoreAll = true;
            return this;
        }

        public ITrackingSaveChangeOptions IgnoreContext<TDbContext>() where TDbContext : DbContext
        {
            if (!_ignoreDbContext.Contains(typeof(TDbContext)))
                _ignoreDbContext.Add(typeof(TDbContext));
            return this;
        }

        public ITrackingSaveChangeOptions IgnoreEntity<TDbContext, TEntity>(EntityTrackingTypes? trackingType = null) where TDbContext : DbContext
        {
            var map = _ignoreEntity.SingleOrDefault(x => x.DbContext.Equals(typeof(TDbContext)) && x.Entity.Equals(typeof(TEntity)));
            if (map != null)
            {
                if (map.TrackingType.HasValue && !map.TrackingType.Value.HasFlag(trackingType))
                    map.TrackingType |= trackingType;
            }
            else
            {
                map = new EntityTrackingTypeIgnoreMap
                {
                    DbContext = typeof(TDbContext),
                    Entity = typeof(TEntity),
                    TrackingType = trackingType
                };

                _ignoreEntity.Add(map);
            }

            return this;
        }
    }
}
