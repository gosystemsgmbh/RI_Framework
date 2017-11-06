using System;

using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Bus.Dispatchers
{
	/// <summary>
	///     Defines the interface for a bus dispatcher.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="IBus" /> for more details about message busses.
	///     </para>
	///     <para>
	///         This interface is part of the actual bus implementation and not intended to be used by the bus users.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public interface IBusDispatcher : ISynchronizable
	{
		/// <summary>
		///     Dispatches a delegate for execution.
		/// </summary>
		/// <param name="action"> The delegate. </param>
		/// <param name="parameters"> Optional parameters of the delegate. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="action" /> is null. </exception>
		void Dispatch (Delegate action, params object[] parameters);

		/// <summary>
		///     Initializes the dispatcher when the bus starts.
		/// </summary>
		/// <param name="dependencyResolver"> The dependency resolver which can be used to get instances of required types. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="dependencyResolver" /> is null. </exception>
		void Initialize (IDependencyResolver dependencyResolver);

		/// <summary>
		///     Unloads the dispatcher when the bus stops.
		/// </summary>
		void Unload ();
	}
}
