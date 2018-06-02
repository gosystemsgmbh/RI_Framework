using System;

using RI.Framework.Composition.Model;
using RI.Framework.StateMachines.States;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.StateMachines.Resolvers
{
	/// <summary>
	///     Implements a state instance resolver which uses an <see cref="IServiceProvider" />.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="IStateResolver" /> for more details.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	[Export]
	public class ServiceProviderStateResolver : IStateResolver
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="ServiceProviderStateResolver" />.
		/// </summary>
		/// <param name="serviceProvider"> The used service provider. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="serviceProvider" /> is null. </exception>
		public ServiceProviderStateResolver (IServiceProvider serviceProvider)
		{
			if (serviceProvider == null)
			{
				throw new ArgumentNullException(nameof(serviceProvider));
			}

			this.ServiceProvider = serviceProvider;

			this.SyncRoot = new object();
		}

		#endregion




		#region Instance Properties/Indexer

		private IServiceProvider ServiceProvider { get; }

		#endregion




		#region Interface: IStateResolver

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		public object SyncRoot { get; }

		/// <inheritdoc />
		public IState ResolveState (Type type)
		{
			lock (this.SyncRoot)
			{
				return this.ServiceProvider.GetService(type) as IState;
			}
		}

		#endregion
	}
}
