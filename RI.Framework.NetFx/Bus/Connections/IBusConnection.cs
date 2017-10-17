using System;

using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Bus.Connections
{
	/// <summary>
	///     Defines the interface for a bus connection.
	/// </summary>
	/// <remarks>
	///     See <see cref="IBus" /> for more details about message busses.
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public interface IBusConnection : ISynchronizable
	{
		/// <summary>
		///     Gets an explanatory message what and why the connection is broken.
		/// </summary>
		/// <value>
		///     An explanatory message what and why the connection is broken or null if the connection is not broken.
		/// </value>
		string BrokenMessage { get; }

		/// <summary>
		///     Gets whether this connection is broken.
		/// </summary>
		/// <value>
		///     true if this connection is broken, false otherwise.
		/// </value>
		bool IsBroken { get; }

		/// <summary>
		///     Initializes the connection when the bus starts.
		/// </summary>
		/// <param name="dependencyResolver"> The dependency resolver which can be used to get instances of required types. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="dependencyResolver" /> is null. </exception>
		void Initialize (IDependencyResolver dependencyResolver);

		/// <summary>
		///     Unloads the connection when the bus stops.
		/// </summary>
		void Unload ();
	}
}
