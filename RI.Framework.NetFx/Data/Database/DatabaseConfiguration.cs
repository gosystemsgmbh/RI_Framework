using System;
using System.Data.Common;

using RI.Framework.Data.Database.Cleanup;
using RI.Framework.Data.Database.Scripts;
using RI.Framework.Data.Database.Upgrading;
using RI.Framework.Data.Database.Versioning;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Data.Database
{
	/// <summary>
	///     Implements the base class for storing the database configuration used by implementations of <see cref="IDatabaseManager" />.
	/// </summary>
	public abstract class DatabaseConfiguration : ICloneable<DatabaseConfiguration>, ICloneable
	{
		#region Instance Constructor/Destructor

		internal DatabaseConfiguration ()
		{
			this.ScriptLocator = null;
			this.VersionDetector = null;
			this.UpgradeProvider = null;
			this.CleanupProvider = null;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets or sets the used cleanup provider.
		/// </summary>
		/// <value>
		///     The used cleanup provider.
		/// </value>
		public IDatabaseCleanupProvider CleanupProvider { get; set; }

		/// <summary>
		///     Gets or sets the used script locator.
		/// </summary>
		/// <value>
		///     The used script locator.
		/// </value>
		public IDatabaseScriptLocator ScriptLocator { get; set; }

		/// <summary>
		///     Gets or sets the used upgrade provider.
		/// </summary>
		/// <value>
		///     The used upgrade provider.
		/// </value>
		public IDatabaseUpgradeProvider UpgradeProvider { get; set; }

		/// <summary>
		///     Gets or sets the used version detector.
		/// </summary>
		/// <value>
		///     The used version detector.
		/// </value>
		public IDatabaseVersionDetector VersionDetector { get; set; }

		#endregion




		#region Abstracts

		internal abstract DatabaseConfiguration CloneInternal ();

		#endregion




		#region Interface: ICloneable<DatabaseConfiguration>

		/// <inheritdoc />
		DatabaseConfiguration ICloneable<DatabaseConfiguration>.Clone ()
		{
			return this.CloneInternal();
		}

		/// <inheritdoc />
		object ICloneable.Clone ()
		{
			return this.CloneInternal();
		}

		#endregion
	}

	/// <summary>
	///     Implements the base class for storing the database configuration used by implementations of <see cref="IDatabaseManager" />.
	/// </summary>
	/// <typeparam name="T">The database configuration type.</typeparam>
	public abstract class DatabaseConfiguration <T> : DatabaseConfiguration, ICloneable<T>
		where T : DatabaseConfiguration<T>, new()
	{
		#region Instance Constructor/Destructor

		internal DatabaseConfiguration()
		{
		}

		#endregion




		#region Virtuals

		/// <summary>
		///     Called when the current instance is to be cloned.
		/// </summary>
		/// <param name="clone"> The clone being created. </param>
		protected virtual void Clone (T clone)
		{
			clone.ScriptLocator = this.ScriptLocator;
			clone.VersionDetector = this.VersionDetector;
			clone.UpgradeProvider = this.UpgradeProvider;
			clone.CleanupProvider = this.CleanupProvider;
		}

		#endregion




		#region Overrides

		internal sealed override DatabaseConfiguration CloneInternal ()
		{
			return this.Clone();
		}

		#endregion




		#region Interface: ICloneable<T>

		/// <inheritdoc />
		public T Clone ()
		{
			T clone = new T();
			this.Clone(clone);
			return clone;
		}

		#endregion
	}

	/// <summary>
	///     Implements the base class for storing the database configuration used by implementations of <see cref="IDatabaseManager" />.
	/// </summary>
	/// <typeparam name="TConnectionStringBuilder">The type of the used connection string builder.</typeparam>
	/// <typeparam name="TConfiguration">The database configuration type.</typeparam>
	public abstract class DatabaseConfiguration<TConnectionStringBuilder, TConfiguration> : DatabaseConfiguration<TConfiguration>
		where TConnectionStringBuilder : DbConnectionStringBuilder, new()
		where TConfiguration : DatabaseConfiguration<TConnectionStringBuilder, TConfiguration>, new()
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="DatabaseConfiguration{TConnectionStringBuilder,TConfiguration}" />.
		/// </summary>
		protected DatabaseConfiguration()
		{
			this.ConnectionString = new TConnectionStringBuilder();
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the connection string builder.
		/// </summary>
		/// <value>
		///     The connection string builder.
		/// </value>
		public TConnectionStringBuilder ConnectionString { get; private set; }

		#endregion




		#region Overrides

		/// <inheritdoc />
		protected override void Clone (TConfiguration clone)
		{
			base.Clone(clone);

			clone.ConnectionString.ConnectionString = this.ConnectionString.ConnectionString;
		}

		#endregion
	}
}
