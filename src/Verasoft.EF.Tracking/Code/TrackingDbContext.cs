using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using System.Data.Common;
using System.Data.Entity.Core.Objects;
using System.Threading;

namespace Verasoft.EF
{
    public class TrackingDbContext : DbContext, ITrackingDbContext
    {
        #region Fields

        private readonly Guid _id = Guid.NewGuid();

        #endregion

        #region Properties

        public Guid ID
        {
            get { return _id; }
        }

        #endregion

        #region Constructor

        public TrackingDbContext() : base()
        {
            this.Initialize();
        }

        public TrackingDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
            this.Initialize();
        }

        public TrackingDbContext(DbCompiledModel model) : base(model)
        {
            this.Initialize();
        }

        public TrackingDbContext(string nameOrConnectionString, DbCompiledModel model)
            : base(nameOrConnectionString, model)
        {
            this.Initialize();
        }

        public TrackingDbContext(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection)
        {
            this.Initialize();
        }

        public TrackingDbContext(ObjectContext objectContext, bool dbContextOwnsObjectContext) : base(objectContext, dbContextOwnsObjectContext)
        {
            this.Initialize();
        }

        public TrackingDbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection) : base(existingConnection, model, contextOwnsConnection)
        {
            this.Initialize();
        }

        #endregion

        #region DbContext

        public override int SaveChanges()
        {
            TrackingContextFactory.Current.BeforeDbContextSaveChanges(this);
            var changes = base.SaveChanges();
            TrackingContextFactory.Current.AfterDbContextSaveChanges(this, CancellationToken.None);
            changes += base.SaveChanges();
            return changes;
        }

        public int SaveChanges(Action<TrackingSaveChangeOptions> action)
        {
            TrackingSaveChangeOptions options = new TrackingSaveChangeOptions();
            action(options);
            TrackingContextFactory.Current.BeforeDbContextSaveChanges(this, options);
            var changes = base.SaveChanges();
            TrackingContextFactory.Current.AfterDbContextSaveChanges(this, CancellationToken.None, options);
            changes += base.SaveChanges();
            return changes;
        }

        public override Task<int> SaveChangesAsync()
        {
            return Task.Factory.StartNew<int>(() =>
            {
                TrackingContextFactory.Current.BeforeDbContextSaveChanges(this);
                var result = base.SaveChangesAsync().Result;
                TrackingContextFactory.Current.AfterDbContextSaveChanges(this, CancellationToken.None);
                base.SaveChangesAsync().Wait();
                return result;
            });
        }

        public Task<int> SaveChangesAsync(Action<TrackingSaveChangeOptions> action)
        {
            TrackingSaveChangeOptions options = new TrackingSaveChangeOptions();
            action(options);

            return Task.Factory.StartNew<int>(() =>
            {
                TrackingContextFactory.Current.BeforeDbContextSaveChanges(this, options);
                var result = base.SaveChangesAsync().Result;
                TrackingContextFactory.Current.AfterDbContextSaveChanges(this, CancellationToken.None, options);
                base.SaveChangesAsync().Wait();
                return result;
            });
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew<int>(() =>
            {
                TrackingContextFactory.Current.BeforeDbContextSaveChanges(this);
                var result = base.SaveChangesAsync(cancellationToken).Result;
                TrackingContextFactory.Current.AfterDbContextSaveChanges(this, cancellationToken);
                base.SaveChangesAsync(cancellationToken).Wait();
                return result;
            }, cancellationToken);
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken, Action<TrackingSaveChangeOptions> action)
        {
            TrackingSaveChangeOptions options = new TrackingSaveChangeOptions();
            action(options);

            return Task.Factory.StartNew<int>(() =>
            {
                TrackingContextFactory.Current.BeforeDbContextSaveChanges(this, options);
                var result = base.SaveChangesAsync(cancellationToken).Result;
                TrackingContextFactory.Current.AfterDbContextSaveChanges(this, cancellationToken, options);
                base.SaveChangesAsync(cancellationToken).Wait();
                return result;
            }, cancellationToken);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            TrackingContextFactory.Current.BeforeDbModelCreation(this, modelBuilder);
            base.OnModelCreating(modelBuilder);
            TrackingContextFactory.Current.AfterDbModelCreation(this, modelBuilder);
        }

        #endregion

        #region Private

        private void Initialize()
        {
            
        }

        #endregion
    }
}
