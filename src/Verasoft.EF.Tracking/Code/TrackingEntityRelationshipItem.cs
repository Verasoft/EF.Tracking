using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Verasoft.EF
{
    public class TrackingEntityRelationshipItem<TEntity, TRelatedEntity> : TrackingEntityItem<TEntity> 
        where TEntity : class
        where TRelatedEntity : class
    {
        public TRelatedEntity RelatedEntity { get; internal set; }
    }
}
