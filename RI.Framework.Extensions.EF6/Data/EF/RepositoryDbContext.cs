using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.Validation;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using RI.Framework.Data.EF.Configuration;
using RI.Framework.Data.EF.Filter;
using RI.Framework.Data.EF.Validation;
using RI.Framework.Data.Repository;
using RI.Framework.Data.Repository.Entities;
using RI.Framework.Services;
using RI.Framework.Services.Logging;




namespace RI.Framework.Data.EF
{
	/// <summary>
	///     Implements the repository / unit-of-work pattern on top of an Entity Frameworks <see cref="DbContext" />.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="IRepositoryContext" /> and <see cref="DbContext" /> for more details.
	///     </para>
	/// </remarks>
	public abstract class RepositoryDbContext : DbContext, IRepositoryContext
	{
		#region Static Constructor/Destructor

		static RepositoryDbContext ()
		{
			RepositoryDbContext.ValidatorsSyncRoot = new object();
			RepositoryDbContext.Validators = new Dictionary<Type, ValidatorCollection>();

			RepositoryDbContext.FiltersSyncRoot = new object();
			RepositoryDbContext.Filters = new Dictionary<Type, FilterCollection>();
		}

		#endregion




		#region Static Properties/Indexer

		private static Dictionary<Type, FilterCollection> Filters { get; set; }

		private static object FiltersSyncRoot { get; set; }

		private static Dictionary<Type, ValidatorCollection> Validators { get; set; }

		private static object ValidatorsSyncRoot { get; set; }

		#endregion




		#region Static Methods

		private static void CreateFilters (RepositoryDbContext repository)
		{
			lock (RepositoryDbContext.FiltersSyncRoot)
			{
				Type repositoryType = repository.GetType();
				if (!RepositoryDbContext.Filters.ContainsKey(repositoryType))
				{
					FilterCollection filters = new FilterCollection();
					FilterRegistrar registrar = new FilterRegistrar(filters);
					repository.OnFiltersCreating(registrar);
					RepositoryDbContext.Filters.Add(repositoryType, filters);
				}
			}
		}

		private static void CreateValidators (RepositoryDbContext repository)
		{
			lock (RepositoryDbContext.ValidatorsSyncRoot)
			{
				Type repositoryType = repository.GetType();
				if (!RepositoryDbContext.Validators.ContainsKey(repositoryType))
				{
					ValidatorCollection validators = new ValidatorCollection();
					ValidationRegistrar registrar = new ValidationRegistrar(validators);
					repository.OnValidatorsCreating(registrar);
					RepositoryDbContext.Validators.Add(repositoryType, validators);
				}
			}
		}

		private static IEntityFilter GetFilter (RepositoryDbContext repository, Type entityType)
		{
			lock (RepositoryDbContext.FiltersSyncRoot)
			{
				RepositoryDbContext.CreateFilters(repository);

				if (RepositoryDbContext.Filters[repository.GetType()].Contains(entityType))
				{
					return RepositoryDbContext.Filters[repository.GetType()][entityType];
				}

				foreach (IEntityFilter filter in RepositoryDbContext.Filters[repository.GetType()])
				{
					if (filter.EntityType.IsAssignableFrom(entityType))
					{
						return filter;
					}
				}

				return null;
			}
		}

		private static IEntityValidation GetValidator (RepositoryDbContext repository, Type entityType)
		{
			lock (RepositoryDbContext.ValidatorsSyncRoot)
			{
				RepositoryDbContext.CreateValidators(repository);

				if (RepositoryDbContext.Validators[repository.GetType()].Contains(entityType))
				{
					return RepositoryDbContext.Validators[repository.GetType()][entityType];
				}

				foreach (IEntityValidation validator in RepositoryDbContext.Validators[repository.GetType()])
				{
					if (validator.EntityType.IsAssignableFrom(entityType))
					{
						return validator;
					}
				}

				return null;
			}
		}

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="RepositoryDbContext" />.
		/// </summary>
		/// <param name="connection"> The database connection to be used by the underlying <see cref="DbContext" />. </param>
		/// <param name="ownConnection"> Specifies whether the underlying <see cref="DbContext" /> owns the connection or not. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="connection" /> is null. </exception>
		public RepositoryDbContext (DbConnection connection, bool ownConnection)
			: base(connection, ownConnection)
		{
			if (connection == null)
			{
				throw new ArgumentNullException(nameof(connection));
			}

			this.EnableDatabaseLogging = true;
			this.FixOnValidateEnabled = true;
			this.FixOnSaveEnabled = true;
			this.EntitySelfErrorTrackingEnabled = true;
			this.EntitySelfChangeTrackingEnabled = true;

			this.Sets = new SetCollection();

			this.Database.CommandTimeout = null;
			this.Database.Log = this.LogDatabase;

			this.Configuration.AutoDetectChangesEnabled = true;
			this.Configuration.EnsureTransactionsForFunctionsAndCommands = true;
			this.Configuration.LazyLoadingEnabled = true;
			this.Configuration.ProxyCreationEnabled = true;
			this.Configuration.UseDatabaseNullSemantics = true;
			this.Configuration.ValidateOnSaveEnabled = true;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets or sets whether database logging is enabled or not.
		/// </summary>
		/// <value>
		///     true if database logging is enabled, false otherwise.
		/// </value>
		/// <remarks>
		///     <para>
		///         The default value is true.
		///     </para>
		///     <para>
		///         See <see cref="LogDatabase" /> for more details.
		///     </para>
		/// </remarks>
		public bool EnableDatabaseLogging { get; set; }

		/// <summary>
		///     Gets or sets whether entity fixing is enabled before pending changes are saved or committed.
		/// </summary>
		/// <value>
		///     true if fixing is enabled, false otherwise.
		/// </value>
		/// <remarks>
		///     <para>
		///         The default value is true.
		///     </para>
		/// </remarks>
		public bool FixOnSaveEnabled { get; set; }

		/// <summary>
		///     Gets or sets whether entity fixing is enabled before validation.
		/// </summary>
		/// <value>
		///     true if fixing is enabled, false otherwise.
		/// </value>
		/// <remarks>
		///     <para>
		///         The default value is true.
		///     </para>
		/// </remarks>
		public bool FixOnValidateEnabled { get; set; }

		private SetCollection Sets { get; set; }

		#endregion




		#region Instance Methods

		/// <summary>
		///     Fixes all entities.
		/// </summary>
		public void FixEntities ()
		{
			this.ChangeTracker.DetectChanges();
			foreach (DbEntityEntry entry in this.ChangeTracker.Entries())
			{
				if (this.ShouldFixEntity(entry))
				{
					this.FixEntity(entry);
				}
			}
		}

		/// <inheritdoc cref="IRepositoryContext.GetSet{T}" />
		public RepositoryDbSet<T> GetSet <T> ()
			where T : class
		{
			return (RepositoryDbSet<T>)this.GetSet(typeof(T));
		}

		/// <inheritdoc cref="IRepositoryContext.GetSet(Type)" />
		public RepositoryDbSet GetSet (Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (!this.Sets.Contains(type))
			{
				this.Sets.Add(this.CreateSetInternal(type));
			}

			return this.Sets[type];
		}

		internal EntityFilter<T> GetFilter <T> ()
			where T : class
		{
			return this.GetFilter(typeof(T)) as EntityFilter<T>;
		}

		internal IEntityFilter GetFilter (Type entityType)
		{
			if (entityType == null)
			{
				throw new ArgumentNullException(nameof(entityType));
			}

			return RepositoryDbContext.GetFilter(this, entityType);
		}

		internal EntityValidation<T> GetValidator <T> ()
			where T : class
		{
			return this.GetValidator(typeof(T)) as EntityValidation<T>;
		}

		internal IEntityValidation GetValidator (Type entityType)
		{
			if (entityType == null)
			{
				throw new ArgumentNullException(nameof(entityType));
			}

			return RepositoryDbContext.GetValidator(this, entityType);
		}

		/// <summary>
		///     Logs a message.
		/// </summary>
		/// <param name="severity"> The severity of the message. </param>
		/// <param name="format"> The message. </param>
		/// <param name="args"> The arguments which will be expanded into the message (comparable to <see cref="string.Format(string, object[])" />). </param>
		/// <remarks>
		///     <para>
		///         <see cref="ILogService" /> is used, obtained through <see cref="ServiceLocator" />.
		///         If no <see cref="ILogService" /> is available, no logging is performed.
		///     </para>
		/// </remarks>
		protected void Log (LogLevel severity, string format, params object[] args)
		{
			LogLocator.Log(severity, this.GetType().Name, format, args);
		}

		private RepositoryDbSet CreateSetInternal (Type type)
		{
			//TODO: Store created method in static dictionary
			MethodInfo method = this.GetType().GetMethod(nameof(this.CreateSet), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			MethodInfo genericMethod = method.MakeGenericMethod(type);
			return (RepositoryDbSet)genericMethod.Invoke(this, null);
		}

		private void PerformEntitySelfChangeTracking ()
		{
			this.ChangeTrackingContext = this.ChangeTrackingContext ?? this.OnChangeTrackingContextResolve();

			DateTime now = DateTime.Now;

			this.ChangeTracker.DetectChanges();
			foreach (DbEntityEntry entry in this.ChangeTracker.Entries())
			{
				IEntityChangeTracking entity = entry.Entity as IEntityChangeTracking;
				if (entity != null)
				{
					if ((entry.State & EntityState.Added) == EntityState.Added)
					{
						entity.SetCreation(this.ChangeTrackingContext, now);
					}

					if ((entry.State & EntityState.Modified) == EntityState.Modified)
					{
						entity.SetModification(this.ChangeTrackingContext, now);
					}
				}
			}
		}

		#endregion




		#region Virtuals

		/// <summary>
		///     Called when a <see cref="RepositoryDbSet{T}" /> is required which does not yet exist.
		/// </summary>
		/// <typeparam name="T"> The type of entity the <see cref="RepositoryDbSet{T}" /> is required for. </typeparam>
		/// <returns>
		///     The <see cref="RepositoryDbSet{T}" /> which manages entities of type <typeparamref name="T" />.
		/// </returns>
		/// <remarks>
		///     <note type="important">
		///         Do not call this method directly to obtain a set, use <see cref="GetSet{T}" /> or <see cref="GetSet(Type)" /> instead.
		///         This method is only used for instantiating the corresponding sets.
		///     </note>
		///     <para>
		///         A set is only created once and then cached and reused for each subsequent call of <see cref="GetSet{T}" /> or <see cref="GetSet(Type)" /> as long as this repository is not disposed.
		///     </para>
		///     <para>
		///         The default implementation creates a new instance of <see cref="RepositoryDbSet{T}" />.
		///     </para>
		/// </remarks>
		protected virtual RepositoryDbSet<T> CreateSet <T> ()
			where T : class
		{
			return new RepositoryDbSet<T>(this, this.EFSet<T>());
		}

		/// <summary>
		///     Creates a <see cref="DbSet" /> by the Entity Framework.
		/// </summary>
		/// <param name="entityType"> The type of entities managed by the <see cref="DbSet" />. </param>
		/// <returns>
		///     The created <see cref="DbSet" />.
		/// </returns>
		/// <remarks>
		///     <note type="important">
		///         Always use this method, never <see cref="DbContext.Set" />!
		///     </note>
		/// </remarks>
		[SuppressMessage("ReSharper", "InconsistentNaming")]
		protected virtual DbSet EFSet (Type entityType) => base.Set(entityType);

		/// <summary>
		///     Creates a <see cref="DbSet{TEntity}" /> by the Entity Framework.
		/// </summary>
		/// <typeparam name="TEntity"> The type of entities managed by the <see cref="DbSet{TEntity}" /> </typeparam>
		/// <returns>
		///     The created <see cref="DbSet{TEntity}" />.
		/// </returns>
		/// <remarks>
		///     <note type="important">
		///         Always use this method, never <see cref="DbContext.Set{TEntity}" />!
		///     </note>
		/// </remarks>
		[SuppressMessage("ReSharper", "InconsistentNaming")]
		protected virtual DbSet<TEntity> EFSet <TEntity> ()
			where TEntity : class => base.Set<TEntity>();

		/// <summary>
		///     Called when a specific entity is to be fixed before it is saved to the database.
		/// </summary>
		/// <param name="entityEntry"> The entity to fix. </param>
		protected virtual void FixEntity (DbEntityEntry entityEntry)
		{
			IEntityValidation validator = this.GetValidator(entityEntry.Entity.GetType());
			validator?.Fix(this, entityEntry);
		}

		/// <summary>
		///     Called when the underlying <see cref="DbContext" /> issues log messages.
		/// </summary>
		/// <param name="message"> The message to log. </param>
		/// <remarks>
		///     <para>
		///         The message is not trimmed before passed to <see cref="LogDatabase" />.
		///     </para>
		///     <para>
		///         The messages coming from <see cref="DbContext" /> are mostly SQL commands sent to the database.
		///     </para>
		///     <para>
		///         The database logging or <see cref="DbContext" /> logging respectively can be enabled/disabled using <see cref="EnableDatabaseLogging" />.
		///     </para>
		///     <para>
		///         The default implementation calls <see cref="Log" /> to log the message.
		///     </para>
		/// </remarks>
		protected virtual void LogDatabase (string message)
		{
			if (this.EnableDatabaseLogging)
			{
				this.Log(LogLevel.Debug, "Database activity: {0}", message.Trim());
			}
		}

		/// <summary>
		///     Tries to resolve the currently used change tracking context for use with <see cref="IEntityChangeTracking" />.
		/// </summary>
		/// <returns>
		///     The resolved change tracking context or null if no change tracking context is available.
		/// </returns>
		protected virtual object OnChangeTrackingContextResolve ()
		{
			ChangeTrackingContextResolveEventArgs eventArgs = new ChangeTrackingContextResolveEventArgs();
			this.ChangeTrackingContextResolve?.Invoke(this, eventArgs);
			return eventArgs.ChangeTrackingContext;
		}

		/// <summary>
		///     Called when the entity configuration is to be created for this repository.
		/// </summary>
		/// <param name="configurations"> The entity configuration registrar to be used. </param>
		/// <remarks>
		///     <para>
		///         The default implementation adds all <see cref="IEntityConfiguration" /> implementations in the assembly of this repositories type.
		///     </para>
		///     <note type="note">
		///         The entity configuration is only created once for a certain type of repository.
		///         It is cached and reused for subsequent instances of the same concrete <see cref="RepositoryDbContext" /> type.
		///     </note>
		/// </remarks>
		protected virtual void OnConfigurationCreating (ConfigurationRegistrar configurations)
		{
			configurations.AddFromAssembly(this.GetType().Assembly);
		}

		/// <summary>
		///     Called when the entity filters are to be created for this repository.
		/// </summary>
		/// <param name="filters"> The entity filter registrar to be used. </param>
		/// <remarks>
		///     <para>
		///         The default implementation adds all <see cref="IEntityFilter" /> implementations in the assembly of this repositories type.
		///     </para>
		///     <note type="note">
		///         The entity filters are only created once for a certain type of repository.
		///         It is cached and reused for subsequent instances of the same concrete <see cref="RepositoryDbContext" /> type.
		///     </note>
		/// </remarks>
		protected virtual void OnFiltersCreating (FilterRegistrar filters)
		{
			filters.AddFromAssembly(this.GetType().Assembly);
		}

		/// <summary>
		///     Called when the entity validations are to be created for this repository.
		/// </summary>
		/// <param name="validators"> The entity validation registrar to be used. </param>
		/// <remarks>
		///     <para>
		///         The default implementation adds all <see cref="IEntityValidation" /> implementations in the assembly of this repositories type.
		///     </para>
		///     <note type="note">
		///         The entity validations are only created once for a certain type of repository.
		///         It is cached and reused for subsequent instances of the same concrete <see cref="RepositoryDbContext" /> type.
		///     </note>
		/// </remarks>
		protected virtual void OnValidatorsCreating (ValidationRegistrar validators)
		{
			validators.AddFromAssembly(this.GetType().Assembly);
		}

		/// <summary>
		///     Determines whether an entity needs to be fixed.
		/// </summary>
		/// <param name="entityEntry"> The entity entry of the entity. </param>
		/// <returns>
		///     true if the entity needs to be fixed, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         By default, all added or modified entities are specified to be fixed.
		///     </para>
		/// </remarks>
		protected virtual bool ShouldFixEntity (DbEntityEntry entityEntry)
		{
			return (entityEntry.State & (EntityState.Added | EntityState.Modified)) != 0;
		}

		#endregion




		#region Overrides

		/// <inheritdoc />
		public override int SaveChanges ()
		{
			if (this.FixOnSaveEnabled)
			{
				this.FixEntities();
			}

			if (this.EntitySelfChangeTrackingEnabled)
			{
				this.PerformEntitySelfChangeTracking();
			}

			return base.SaveChanges();
		}

		/// <inheritdoc />
		public override Task<int> SaveChangesAsync (CancellationToken cancellationToken)
		{
			if (this.FixOnSaveEnabled)
			{
				this.FixEntities();
			}

			if (this.EntitySelfChangeTrackingEnabled)
			{
				this.PerformEntitySelfChangeTracking();
			}

			return base.SaveChangesAsync(cancellationToken);
		}

		/// <inheritdoc />
		public sealed override Task<int> SaveChangesAsync () => base.SaveChangesAsync();

		/// <inheritdoc />
		public sealed override DbSet Set (Type entityType)
		{
			RepositoryDbSet set = this.GetSet(entityType);
			return set.Set;
		}

		/// <inheritdoc />
		public sealed override DbSet<TEntity> Set <TEntity> ()
		{
			RepositoryDbSet<TEntity> set = this.GetSet<TEntity>();
			return set.Set;
		}

		/// <inheritdoc />
		protected override void Dispose (bool disposing)
		{
			base.Dispose(disposing);

			this.Sets.Clear();

			this.Database.Log = null;
		}

		/// <inheritdoc />
		protected override void OnModelCreating (DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			this.OnConfigurationCreating(modelBuilder.Configurations);
		}

		/// <inheritdoc />
		protected override DbEntityValidationResult ValidateEntity (DbEntityEntry entityEntry, IDictionary<object, object> items)
		{
			if (this.FixOnValidateEnabled && this.ShouldFixEntity(entityEntry))
			{
				this.FixEntity(entityEntry);
			}

			IEntityValidation validator = this.GetValidator(entityEntry.Entity.GetType());
			DbEntityValidationResult result = validator?.Validate(this, entityEntry);

			if (this.EntitySelfErrorTrackingEnabled)
			{
				(entityEntry.Entity as IEntityErrorTracking)?.SetErrors(result?.ToRepositoryErrors());
			}

			return result ?? base.ValidateEntity(entityEntry, items);
		}

		#endregion




		#region Interface: IRepositoryContext

		/// <inheritdoc />
		public object ChangeTrackingContext { get; set; }

		/// <inheritdoc />
		public bool EntitySelfChangeTrackingEnabled { get; set; }

		/// <inheritdoc />
		public bool EntitySelfErrorTrackingEnabled { get; set; }

		/// <inheritdoc />
		public event EventHandler<ChangeTrackingContextResolveEventArgs> ChangeTrackingContextResolve;

		/// <inheritdoc />
		public void Commit ()
		{
			this.SaveChanges();
			this.Dispose();
		}

		/// <inheritdoc />
		IRepositorySet IRepositoryContext.GetSet (Type type) => this.GetSet(type);

		/// <inheritdoc />
		IRepositorySet<T> IRepositoryContext.GetSet <T> () => this.GetSet<T>();

		/// <inheritdoc />
		public bool HasChanges ()
		{
			return this.ChangeTracker.HasChanges();
		}

		/// <inheritdoc />
		public bool HasErrors ()
		{
			return this.GetValidationErrors().ToList().Any(x => !x.IsValid);
		}

		/// <inheritdoc />
		public void Rollback ()
		{
			this.Dispose();
		}

		/// <inheritdoc />
		void IRepositoryContext.SaveChanges () => this.SaveChanges();

		#endregion




		#region Type: FilterCollection

		private sealed class FilterCollection : KeyedCollection<Type, IEntityFilter>
		{
			#region Overrides

			protected override Type GetKeyForItem (IEntityFilter item)
			{
				return item.EntityType;
			}

			#endregion
		}

		#endregion




		#region Type: SetCollection

		private sealed class SetCollection : KeyedCollection<Type, RepositoryDbSet>
		{
			#region Overrides

			protected override Type GetKeyForItem (RepositoryDbSet item)
			{
				return item.EntityType;
			}

			#endregion
		}

		#endregion




		#region Type: ValidatorCollection

		private sealed class ValidatorCollection : KeyedCollection<Type, IEntityValidation>
		{
			#region Overrides

			protected override Type GetKeyForItem (IEntityValidation item)
			{
				return item.EntityType;
			}

			#endregion
		}

		#endregion
	}
}
