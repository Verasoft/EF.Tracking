using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Verasoft.EF
{
    public class TrackingSubscription<TDbContext> : ITrackingSubscription where TDbContext : DbContext
    {
        #region Properties

        internal TrackingSubscriber<TDbContext> Subscriber { get; set; }
        public TrackingEvent Event { get; set; }
        public Delegate Action { get; set; }
        public bool Handle { get; protected set; }
        public bool Running { get; internal set; }

        #endregion

        #region Constructor

        public TrackingSubscription(TrackingSubscriber<TDbContext> subscriber)
        {
            this.Subscriber = subscriber;
        }

        #endregion

        public virtual void Verify(DbContext context)
        {
            this.Handle = this.Action != null;
        }

        public virtual void Perform(DbContext context)
        {
            try
            {
                if (this.Handle)
                {
                    this.Running = true;
                    this.Action.DynamicInvoke(context);
                }
            }
            finally
            {
                this.Running = false;
                this.Handle = false;
            }
        }

        public bool IsForDbContext(DbContext context)
        {
            return ((object)context).GetType().Equals(typeof(TDbContext));
        }

        public virtual bool IsIgnored(TrackingSaveChangeOptions options)
        {
            if (options == null)
                return false;

            if (options._ignoreAll)
                return true;

            if (options._ignoreDbContext.Contains(typeof(TDbContext)))
                return true;

            return false;
        }
    }

    public class TrackingSubscription<TDbContext, TEntity> : TrackingSubscription<TDbContext>, ITrackingSubscription<TEntity>
        where TDbContext : DbContext
        where TEntity : class
    {
        internal IDictionary<DbEntityEntry<TEntity>, TrackingEntityItem<TEntity>> EntityItems { get; set; }

        public EntityTrackingTypes TrackingType { get; set; }
        
        //Property
        public Expression<Func<TEntity, object>> Property { get; set; }
        public object PropertyValue { get; set; }


        public TrackingSubscription(TrackingSubscriber<TDbContext> subscriber, TrackingSubscription<TDbContext> subscription = null) : base(subscriber)
        {
            if (subscription != null)
            {
                this.Action = subscription.Action;
                this.Event = subscription.Event;
                this.Handle = subscription.Handle;
            }

            this.EntityItems = new Dictionary<DbEntityEntry<TEntity>, TrackingEntityItem<TEntity>>();
        }

        public override void Verify(DbContext context)
        {
            this.Handle = false;
            this.EntityItems.Clear();

            foreach (var entry in context.ChangeTracker.Entries<TEntity>().Where(x => IsTracked(x)))
            {
                if (this.Property != null)
                {
                    PropertyInfo property = null;
                    object originalValue = null;
                    object currentValue = null;

                    if (IsPropertyChange(entry, out property, out originalValue, out currentValue))
                    {
                        this.EntityItems.Add(entry, new TrackingEntityPropertyChangedItem<TEntity>
                        {
                            Entity = entry.Entity,
                            Property = property,
                            OriginalValue = originalValue,
                            CurrentValue = currentValue
                        });    
                    }
                }
                else
                {
                    this.EntityItems.Add(entry, new TrackingEntityItem<TEntity>
                    {
                        Entity = entry.Entity
                    });
                }
            }

            this.Handle = this.EntityItems.Count > 0;
        }

        public override void Perform(DbContext context)
        {
            try
            {
                if (this.Handle)
                {
                    this.Running = true;
                    foreach (var entry in this.EntityItems)
                    {
                        this.Action.DynamicInvoke(context, entry.Value);
                    }
                }
            }
            finally
            {
                this.Running = false;
                this.Handle = false;
            }
        }

        private bool IsTracked(DbEntityEntry<TEntity> entry)
        {
            EntityTrackingTypes type = EntityTrackingTypes.Added | EntityTrackingTypes.Deleted | EntityTrackingTypes.Modified;

            if (IsTrackingType(entry, out type))
            {
                return this.TrackingType.HasFlag(type);
            }

            return false;
        }

        private bool IsTrackingType(DbEntityEntry<TEntity> entry, out EntityTrackingTypes type)
        {
            type = EntityTrackingTypes.Added | EntityTrackingTypes.Deleted | EntityTrackingTypes.Modified;

            switch (entry.State)
            {
                case EntityState.Added:
                    type = EntityTrackingTypes.Added;
                    return true;
                case EntityState.Deleted:
                    type = EntityTrackingTypes.Deleted;
                    return true;
                case EntityState.Modified:
                    type = EntityTrackingTypes.Modified;
                    return true;
            }

            return false;
        }

        private bool IsPropertyChange(DbEntityEntry<TEntity> entry, out PropertyInfo property, out object originalValue, out object currentValue)
        {
            property = null;
            originalValue = null;
            currentValue = null;

            if (this.Property != null)
            {
                var member = this.Property.Body as MemberExpression;
                if (member != null)
                {
                    property = member.Member as PropertyInfo;
                    if (property != null)
                    {
                        if (entry.OriginalValues.PropertyNames.Contains(property.Name))
                        {
                            originalValue = entry.OriginalValues[property.Name];
                            currentValue = entry.CurrentValues[property.Name];

                            if (this.PropertyValue != null)
                            {
                                return currentValue.Equals(this.PropertyValue);
                            }
                            else
                            {
                                if (originalValue == null && currentValue == null)
                                    return false;

                                if (originalValue == null && currentValue != null)
                                    return true;

                                if (originalValue != null && currentValue == null)
                                    return true;

                                if (!originalValue.Equals(currentValue))
                                    return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public override bool IsIgnored(TrackingSaveChangeOptions options)
        {
            if (options == null)
                return false;

            if (base.IsIgnored(options))
                return true;

            var map = options._ignoreEntity.SingleOrDefault(x => x.DbContext.Equals(typeof(TDbContext)) && x.Entity.Equals(typeof(TEntity)));
            if (map != null)
            {
                if (!map.TrackingType.HasValue)
                    return true;

                if (map.TrackingType.Value.HasFlag(this.TrackingType))
                    return true;
            }

            return false;
        }
    }

    public class TrackingSubscription<TDbContext, TEntity, TRelatedEntity> : TrackingSubscription<TDbContext, TEntity>, ITrackingSubscription<TEntity, TRelatedEntity>
        where TDbContext : DbContext
        where TEntity : class
        where TRelatedEntity : class
    {
        internal IList<TrackingEntityRelationshipItem<TEntity, TRelatedEntity>> RelatedItems { get; set; }

        #region Properties

        public Expression<Func<TEntity, System.Collections.Generic.ICollection<TRelatedEntity>>> RelatedEntitySet { get; set; }
        public RelatedEntityTrackingTypes RelatedEntityTrackingType { get; set; }

        #endregion

        #region Constructor

        public TrackingSubscription(TrackingSubscriber<TDbContext> subscriber, TrackingSubscription<TDbContext, TEntity> subscription = null)
            : base(subscriber)
        {
            if (subscription != null)
            {
                this.Action = subscription.Action;
                this.Event = subscription.Event;
                this.Handle = subscription.Handle;
                this.EntityItems = subscription.EntityItems;
                this.Property = subscription.Property;
                this.PropertyValue = subscription.PropertyValue;
                this.TrackingType = subscription.TrackingType;
            }

            this.RelatedItems = new List<TrackingEntityRelationshipItem<TEntity, TRelatedEntity>>();
        }

        #endregion

        #region TrackingSubscription<TEntity>

        public override void Verify(DbContext context)
        {
            base.Verify(context);

            var objContext = ((IObjectContextAdapter)context).ObjectContext;

            var relationshipEntries = objContext.ObjectStateManager
                                                .GetObjectStateEntries(GetEntityState(this.RelatedEntityTrackingType))
                                                .Where(x => x.IsRelationship);

            foreach (var relationshipEntry in relationshipEntries)
            {
                TEntity entity = default(TEntity);
                TRelatedEntity relatedEntity = default(TRelatedEntity);

                if (IsEntityRelationship(relationshipEntry, out entity, out relatedEntity))
                {
                    RelatedItems.Add(new TrackingEntityRelationshipItem<TEntity, TRelatedEntity> { Entity = entity, RelatedEntity = relatedEntity });
                }
            }

            this.Handle |= this.RelatedItems.Count > 0;
        }

        public override void Perform(DbContext context)
        {
            try
            {
                if (this.Handle)
                {
                    base.Perform(context);
                    this.Running = true;

                    foreach (var item in this.RelatedItems)
                    {
                        this.Action.DynamicInvoke(context, item);
                    }
                }
            }
            finally
            {
                this.Running = false;
                this.Handle = false;
            }
        }

        #endregion

        private bool IsEntityRelationship(ObjectStateEntry relationshipEntry, out TEntity entity, out TRelatedEntity relatedEntity)
        {
            entity = default(TEntity);
            relatedEntity = default(TRelatedEntity);

            if (relationshipEntry.IsRelationship)
            {
                var relation1Entry = GetEntityEntryFromRelation(relationshipEntry, 0);
                var relation2Entry = GetEntityEntryFromRelation(relationshipEntry, 1);

                bool? relationshipEntitySetMatch = null;
                bool isManyToMany = relation1Entry.EntitySet != null & relation2Entry.EntitySet != null;
                int entityRelation = 0;

                if (this.RelatedEntitySet != null)
                {
                    string relatedEntitySetName = (this.RelatedEntitySet.Body as MemberExpression).Member.Name;
                    if (relation1Entry.EntitySet != null && relation1Entry.EntitySet.Name.Equals(relatedEntitySetName) &&
                        relation1Entry.Entity is TRelatedEntity && relation2Entry.Entity is TEntity)
                    {
                        entityRelation = 2;
                        relationshipEntitySetMatch = true;
                    }
                    else if (relation2Entry.EntitySet != null && relation2Entry.EntitySet.Name.Equals(relatedEntitySetName) &&
                        relation1Entry.Entity is TEntity && relation2Entry.Entity is TRelatedEntity)
                    {
                        entityRelation = 1;
                        relationshipEntitySetMatch = true;
                    }
                    else
                    {
                        relationshipEntitySetMatch = false;
                    }
                }
                else
                {
                    if (relation1Entry.Entity is TEntity && relation2Entry.Entity is TRelatedEntity)
                    {
                        entityRelation = 1;
                    }
                    else if (relation1Entry.Entity is TRelatedEntity && relation2Entry.Entity is TEntity)
                    {
                        entityRelation = 2;
                    }
                }

                if (!relationshipEntitySetMatch.HasValue || relationshipEntitySetMatch == true)
                {
                    if (entityRelation == 1)
                    {
                        entity = (TEntity)relation1Entry.Entity;
                        relatedEntity = (TRelatedEntity)relation2Entry.Entity;
                        return true;
                    }
                    
                    if (entityRelation == 2)
                    {
                        entity = (TEntity)relation2Entry.Entity;
                        relatedEntity = (TRelatedEntity)relation1Entry.Entity;
                        return true;
                    }
                }
            }

            return false;
        }

        private ObjectStateEntry GetEntityEntryFromRelation(ObjectStateEntry relationEntry, int index)
        {
            var firstKey = relationEntry.State == EntityState.Added ?
                (EntityKey)relationEntry.CurrentValues[index] :
                (EntityKey)relationEntry.OriginalValues[index];

            return relationEntry.ObjectStateManager.GetObjectStateEntry(firstKey);
        }

        private EntityState GetEntityState(RelatedEntityTrackingTypes type)
        {
            EntityState state = EntityState.Unchanged;

            if (type.HasFlag(RelatedEntityTrackingTypes.Added))
                state |= EntityState.Added;
            if (type.HasFlag(RelatedEntityTrackingTypes.Deleted))
                state |= EntityState.Deleted;

            state &= ~EntityState.Unchanged;

            return state;
        }
    }

}
