using System;

using RI.Framework.Utilities.ObjectModel;

namespace RI.Framework.Bus.Routers
{
	public interface IBusRouter
	{
		/// <summary>
		/// Initializes the router when the bus starts.
		/// </summary>
		/// <param name="dependencyResolver">The dependency resolver which can be used to get instances of required types.</param>
		/// <exception cref="ArgumentNullException"><paramref name="dependencyResolver"/> is null.</exception>
		void Initialize(IDependencyResolver dependencyResolver);

		/// <summary>
		/// Unloads the router when the bus stops.
		/// </summary>
		void Unload();
	}
}
