using System;

using RI.Framework.ComponentModel;
using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Bus.Pipeline
{
	/// <summary>
	///     Defines the interface for a bus processing pipeline.
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
	public interface IBusPipeline : ISynchronizable, ILogSource
	{
		/// <summary>
		///     Processes all pending work on the bus processing pipeline.
		/// </summary>
		/// <param name="polling"> Specifies whether <see cref="DoWork" /> is called by polling (true) or because actual work is available (false). </param>
		/// <remarks>
		///     <para>
		///         <see cref="DoWork" /> is also called in the interval of <see cref="IBus.PollInterval" /> when no work is available.
		///     </para>
		/// </remarks>
		void DoWork (bool polling);

		/// <summary>
		///     Initializes the pipeline when the bus starts.
		/// </summary>
		/// <param name="dependencyResolver"> The dependency resolver which can be used to get instances of required types. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="dependencyResolver" /> is null. </exception>
		void Initialize (IDependencyResolver dependencyResolver);

		/// <summary>
		///     Unloads the pipeline when the bus stops.
		/// </summary>
		void Unload ();
	}
}
