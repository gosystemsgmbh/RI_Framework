using System;

using RI.Framework.Bus.Connections;

namespace RI.Framework.Bus
{
	/// <summary>
	///     Event arguments for connection-related <see cref="IBus" /> events.
	/// </summary>
	[Serializable]
	public sealed class BusConnectionEventArgs : EventArgs
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="BusConnectionEventArgs" />.
		/// </summary>
		/// <param name="connection"> The connection. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="connection" /> is null. </exception>
		public BusConnectionEventArgs(IBusConnection connection)
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
		///     Gets the connection.
		/// </summary>
		/// <value>
		///     The connection.
		/// </value>
		public IBusConnection Connection { get; }

		#endregion
	}
}
