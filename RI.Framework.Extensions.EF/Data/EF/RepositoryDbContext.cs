﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.Validation;
using System.Linq;

using RI.Framework.Data.EF.Validation;
using RI.Framework.Data.Repository;
using RI.Framework.Services;
using RI.Framework.Services.Logging;




namespace RI.Framework.Data.EF
{
	/// <summary>
	/// Implements the repository / unit-of-work pattern on top of an Entity Frameworks <see cref="DbContext"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// See <see cref="IRepositoryContext"/> and <see cref="DbContext"/> for more details.
	/// </para>
	/// </remarks>
	public class RepositoryDbContext : DbContext, IRepositoryContext
	{
		#region Static Constructor/Destructor

		static RepositoryDbContext ()
		{
			RepositoryDbContext.ValidatorsSyncRoot = new object();
			RepositoryDbContext.Validators = new Dictionary<Type, ValidatorCollection>();
		}

		#endregion




		#region Static Properties/Indexer

		private static Dictionary<Type, ValidatorCollection> Validators { get; set; }

		private static object ValidatorsSyncRoot { get; set; }

		#endregion




		#region Static Methods

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

		private static IEntityValidation GetValidator (RepositoryDbContext repository, Type entityType)
		{
			lock (RepositoryDbContext.ValidatorsSyncRoot)
			{
				RepositoryDbContext.CreateValidators(repository);

				if (!RepositoryDbContext.Validators[repository.GetType()].Contains(entityType))
				{
					return null;
				}

				return RepositoryDbContext.Validators[repository.GetType()][entityType];
			}
		}

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		/// Creates a new instance of <see cref="RepositoryDbContext"/>.
		/// </summary>
		/// <param name="connection">The database connection to be used by the underlying <see cref="DbContext"/>.</param>
		/// <param name="ownConnection">Specifies whether the underlying <see cref="DbContext"/> owns the connection or not.</param>
		/// <exception cref="ArgumentNullException"><paramref name="connection"/> is null.</exception>
		public RepositoryDbContext (DbConnection connection, bool ownConnection)
			: base(connection, ownConnection)
		{
			if (connection == null)
			{
				throw new ArgumentNullException(nameof(connection));
			}

			this.EnableDatabaseLogging = true;
			this.Sets = new SetCollection();

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

		private SetCollection Sets { get; set; }

		/// <summary>
		/// Gets or sets whether database logging is enabled or not.
		/// </summary>
		/// <value>
		/// true if database logging is enabled, false otherwise.
		/// </value>
		/// <remarks>
		/// <para>
		/// The default value is true.
		/// </para>
		/// <para>
		/// See <see cref="LogDatabase"/> for more details.
		/// </para>
		/// </remarks>
		public bool EnableDatabaseLogging { get; set; }

		#endregion




		#region Instance Methods

		/// <inheritdoc cref="IRepositoryContext.GetSet{T}"/>
		public RepositoryDbSet<T> GetSet <T> () where T : class
		{
			Type entityType = typeof(T);
			if (!this.Sets.Contains(entityType))
			{
				this.Sets.Add(this.CreateSet<T>());
			}
			return (RepositoryDbSet<T>)this.Sets[entityType];
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

		#endregion




		#region Virtuals

		/// <summary>
		/// Called when a <see cref="RepositoryDbSet{T}"/> is required which does not yet exist.
		/// </summary>
		/// <typeparam name="T">The type of entity the <see cref="RepositoryDbSet{T}"/> is required for.</typeparam>
		/// <returns>
		/// The <see cref="RepositoryDbSet{T}"/> which manages entities of type <typeparamref name="T"/>.
		/// </returns>
		/// <remarks>
		/// <note type="important">
		/// Do not call this method directly to obtain a set, use <see cref="GetSet{T}"/> instead.
		/// THis method is only used for instantiating the corresponding sets.
		/// </note>
		/// <para>
		/// A set is only created once and then cached, reused for each subsequent call of <see cref="GetSet{T}"/> as long as this repository is not disposed.
		/// </para>
		/// <para>
		/// The default implementation creates a new instance of <see cref="RepositoryDbSet{T}"/>.
		/// </para>
		/// </remarks>
		protected virtual RepositoryDbSet<T> CreateSet <T> ()
			where T : class
		{
			return new RepositoryDbSet<T>(this, this.Set<T>());
		}

		/// <summary>
		/// Called when the underlying <see cref="DbContext"/> issues log messages.
		/// </summary>
		/// <param name="message">The message to log.</param>
		/// <remarks>
		/// <para>
		/// The message is not trimmed before passed to <see cref="LogDatabase"/>.
		/// </para>
		/// <para>
		/// The messages coming from <see cref="DbContext"/> are mostly SQL commands sent to the database.
		/// </para>
		/// <para>
		/// The database logging or <see cref="DbContext"/> logging respectively can be enabled/disabled using <see cref="EnableDatabaseLogging"/>.
		/// </para>
		/// <para>
		/// The default implementation calls <see cref="Log"/> to log the message.
		/// </para>
		/// </remarks>
		protected virtual void LogDatabase (string message)
		{
			this.Log(LogLevel.Debug, "Database activity: {0}", message.Trim());
		}

		/// <summary>
		/// Called when the entity configuration is to be created for this repository.
		/// </summary>
		/// <param name="configurations">The entity configuration registrar to be used.</param>
		/// <remarks>
		/// <note type="note">
		/// The entity configuration is only created once for a certain type of repository.
		/// It is cached and reused for subsequent instances of the same concrete <see cref="RepositoryDbContext"/> type.
		/// </note>
		/// </remarks>
		protected virtual void OnConfigurationCreating (ConfigurationRegistrar configurations)
		{
			configurations.AddFromAssembly(this.GetType().Assembly);
		}

		/// <summary>
		/// Called when the entity validation is to be created for this repository.
		/// </summary>
		/// <param name="validators">The entity validation registrar to be used.</param>
		/// <remarks>
		/// <note type="note">
		/// The entity validation is only created once for a certain type of repository.
		/// It is cached and reused for subsequent instances of the same concrete <see cref="RepositoryDbContext"/> type.
		/// </note>
		/// </remarks>
		protected virtual void OnValidatorsCreating (ValidationRegistrar validators)
		{
			validators.AddFromAssembly(this.GetType().Assembly);
		}

		#endregion




		#region Overrides

		/// <inheritdoc />
		protected override void Dispose (bool disposing)
		{
			this.Database.Log = null;
			this.Sets.Clear();

			base.Dispose(disposing);
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
			IEntityValidation validator = RepositoryDbContext.GetValidator(this, entityEntry.Entity.GetType());
			DbEntityValidationResult result = validator?.Validate(this, entityEntry);
			return result ?? base.ValidateEntity(entityEntry, items);
		}

		#endregion




		#region Interface: IRepositoryContext

		/// <inheritdoc />
		public void Commit ()
		{
			this.SaveChanges();
			this.Dispose();
		}

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




		#region Type: SetCollection

		private sealed class SetCollection : KeyedCollection<Type, IRepositoryDbSet>
		{
			#region Overrides

			protected override Type GetKeyForItem (IRepositoryDbSet item)
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
