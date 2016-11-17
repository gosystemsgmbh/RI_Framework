using System;
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
				return RepositoryDbContext.Validators[repository.GetType()][entityType];
			}
		}

		#endregion




		#region Instance Constructor/Destructor

		public RepositoryDbContext (DbConnection connection, bool ownConnection)
			: base(connection, ownConnection)
		{
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

		#endregion




		#region Instance Methods

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

		protected virtual RepositoryDbSet<T> CreateSet <T> ()
			where T : class
		{
			return new RepositoryDbSet<T>(this, this.Set<T>());
		}

		protected virtual void LogDatabase (string message)
		{
			this.Log(LogLevel.Debug, "Database activity: {0}", message.Trim());
		}

		protected virtual void OnConfigurationCreating (ConfigurationRegistrar configurations)
		{
			configurations.AddFromAssembly(this.GetType().Assembly);
		}

		protected virtual void OnValidatorsCreating (ValidationRegistrar validators)
		{
			validators.AddFromAssembly(this.GetType().Assembly);
		}

		#endregion




		#region Overrides

		protected override void Dispose (bool disposing)
		{
			this.Database.Log = null;
			this.Sets.Clear();

			base.Dispose(disposing);
		}

		protected override void OnModelCreating (DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			this.OnConfigurationCreating(modelBuilder.Configurations);
		}

		protected override DbEntityValidationResult ValidateEntity (DbEntityEntry entityEntry, IDictionary<object, object> items)
		{
			IEntityValidation validator = RepositoryDbContext.GetValidator(this, entityEntry.Entity.GetType());
			DbEntityValidationResult result = validator?.Validate(this, entityEntry);
			return result ?? base.ValidateEntity(entityEntry, items);
		}

		#endregion




		#region Interface: IRepositoryContext

		public void Commit ()
		{
			this.SaveChanges();
			this.Dispose();
		}

		IRepositorySet<T> IRepositoryContext.GetSet <T> () => this.GetSet<T>();

		public bool HasChanges ()
		{
			return this.ChangeTracker.HasChanges();
		}

		public bool HasErrors ()
		{
			return this.GetValidationErrors().ToList().Any(x => !x.IsValid);
		}

		public void Rollback ()
		{
			this.Dispose();
		}

		public void RollbackErrors ()
		{
			List<DbEntityValidationResult> validationResults = this.GetValidationErrors().ToList();
			foreach (DbEntityValidationResult validationResult in validationResults)
			{
				if (!validationResult.IsValid)
				{
					validationResult.Entry.Reload();
				}
			}
		}

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
