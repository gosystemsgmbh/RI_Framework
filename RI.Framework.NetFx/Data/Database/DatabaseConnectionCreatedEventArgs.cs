using System;
using System.Data;
using System.Data.Common;

namespace RI.Framework.Data.Database
{
	/// <summary>
	///     Event arguments for the <see cref="IDatabaseManager" />.<see cref="IDatabaseManager.ConnectionCreated" /> event.
	/// </summary>
	[Serializable]
	public abstract class DatabaseConnectionCreatedEventArgs : EventArgs
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="DatabaseConnectionCreatedEventArgs" />.
		/// </summary>
		/// <param name="connection"> The connection which changed its state. </param>
		internal DatabaseConnectionCreatedEventArgs(DbConnection connection)
		{
			if (connection == null)
			{
				throw new ArgumentNullException(nameof(connection));
			}

			this.Connection = connection;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the connection which changed its state.
		/// </summary>
		/// <value>
		///     The connection which changed its state.
		/// </value>
		public DbConnection Connection { get; }

		#endregion
	}

	/// <summary>
	///     Event arguments for the <see cref="IDatabaseManager" />.<see cref="IDatabaseManager{TConnection,TConnectionStringBuilder,TManager}.ConnectionCreated" /> event.
	/// </summary>
	/// <typeparam name="TConnection">The type of database connections.</typeparam>
	[Serializable]
	public sealed class DatabaseConnectionCreatedEventArgs<TConnection> : DatabaseConnectionCreatedEventArgs
		where TConnection : DbConnection
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="DatabaseConnectionCreatedEventArgs{TConnection}" />.
		/// </summary>
		/// <param name="connection"> The connection which changed its state. </param>
		public DatabaseConnectionCreatedEventArgs(TConnection connection)
			: base(connection)
		{
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the connection which changed its state.
		/// </summary>
		/// <value>
		///     The connection which changed its state.
		/// </value>
		public new TConnection Connection => (TConnection)base.Connection;

		#endregion
	}
}
