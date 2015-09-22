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
using Microsoft.AspNet.Identity.EntityFramework;
using Verasoft.EF;

namespace Verasoft.EF.Identity
{
    public class TrackingIdentityDbContext : IdentityDbContext, ITrackingDbContext
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

        public TrackingIdentityDbContext() : base()
        {
            this.Initialize();
        }

        public TrackingIdentityDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
            this.Initialize();
        }

        public TrackingIdentityDbContext(DbCompiledModel model) : base(model)
        {
            this.Initialize();
        }

        public TrackingIdentityDbContext(string nameOrConnectionString, DbCompiledModel model)
            : base(nameOrConnectionString, model)
        {
            this.Initialize();
        }

        public TrackingIdentityDbContext(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection)
        {
            this.Initialize();
        }

        public TrackingIdentityDbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection) : base(existingConnection, model, contextOwnsConnection)
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

        #endregion

        #region Private

        private void Initialize()
        {
            
        }

        #endregion
    }

    public class TrackingIdentityDbContext<TUser> : IdentityDbContext<TUser>, ITrackingDbContext where TUser : Microsoft.AspNet.Identity.EntityFramework.IdentityUser
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

        public TrackingIdentityDbContext() : base()
        {
            this.Initialize();
        }

        public TrackingIdentityDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
            this.Initialize();
        }

        public TrackingIdentityDbContext(string nameOrConnectionString, bool throwIfV1Schema)
            : base(nameOrConnectionString, throwIfV1Schema)
        {
            this.Initialize();
        }

        public TrackingIdentityDbContext(DbCompiledModel model) : base(model)
        {
            this.Initialize();
        }

        public TrackingIdentityDbContext(string nameOrConnectionString, DbCompiledModel model)
            : base(nameOrConnectionString, model)
        {
            this.Initialize();
        }

        public TrackingIdentityDbContext(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection)
        {
            this.Initialize();
        }

        public TrackingIdentityDbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
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

        #endregion

        #region Private

        private void Initialize()
        {
            
        }

        #endregion
    }

    public class TrackingIdentityDbContext<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim> : IdentityDbContext<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim>,
        ITrackingDbContext
        where TUser : Microsoft.AspNet.Identity.EntityFramework.IdentityUser<TKey, TUserLogin, TUserRole, TUserClaim>
        where TRole : Microsoft.AspNet.Identity.EntityFramework.IdentityRole<TKey, TUserRole>
        where TUserLogin : Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<TKey>
        where TUserRole : Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<TKey>
        where TUserClaim : Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<TKey>
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

        public TrackingIdentityDbContext()
            : base()
        {
            this.Initialize();
        }

        public TrackingIdentityDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            this.Initialize();
        }

        public TrackingIdentityDbContext(DbCompiledModel model)
            : base(model)
        {
            this.Initialize();
        }

        public TrackingIdentityDbContext(string nameOrConnectionString, DbCompiledModel model)
            : base(nameOrConnectionString, model)
        {
            this.Initialize();
        }

        public TrackingIdentityDbContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
            this.Initialize();
        }

        public TrackingIdentityDbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
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

        #endregion

        #region Private

        private void Initialize()
        {

        }

        #endregion
    }
}
