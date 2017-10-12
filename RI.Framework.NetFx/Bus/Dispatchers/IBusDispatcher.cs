using System;

using RI.Framework.Utilities.ObjectModel;

namespace RI.Framework.Bus.Connections
{
	public interface IBusDispatcher
	{
		/// <summary>
		/// Initializes the dispatcher when the bus starts.
		/// </summary>
		/// <param name="dependencyResolver">The dependency resolver which can be used to get instances of required types.</param>
		/// <exception cref="ArgumentNullException"><paramref name="dependencyResolver"/> is null.</exception>
		void Initialize(IDependencyResolver dependencyResolver);

		/// <summary>
		/// Unloads the dispatcher when the bus stops.
		/// </summary>
		void Unload();
	}
}
