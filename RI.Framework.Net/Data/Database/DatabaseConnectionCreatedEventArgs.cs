using System;
using System.Data.Common;




namespace RI.Framework.Data.Database
{
	/// <summary>
	///     Event arguments for the <see cref="IDatabaseManager" />.<see cref="IDatabaseManager.ConnectionCreated" /> event.
	/// </summary>
	[Serializable]
	public sealed class DatabaseConnectionCreatedEventArgs : EventArgs
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="DatabaseConnectionCreatedEventArgs" />.
		/// </summary>
		/// <param name="connection"> The created connection. </param>
		/// <param name="isReadOnly"> Specifies whether the connection is read-only or not. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="connection" /> is null. </exception>
		public DatabaseConnectionCreatedEventArgs (DbConnection connection, bool isReadOnly)
		{
			if (connection == null)
			{
				throw new ArgumentNullException(nameof(connection));
			}

			this.Connection = connection;
			this.IsReadOnly = isReadOnly;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the created connection.
		/// </summary>
		/// <value>
		///     The created connection.
		/// </value>
		public DbConnection Connection { get; }

		/// <summary>
		///     Gets whether the connection is read-only or not.
		/// </summary>
		/// <value>
		///     true if the connection is read-only, false otherwise.
		/// </value>
		public bool IsReadOnly { get; }

		#endregion
	}
}
