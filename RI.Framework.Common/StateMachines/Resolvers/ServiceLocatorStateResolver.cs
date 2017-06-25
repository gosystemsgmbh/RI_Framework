using System;

using RI.Framework.Services;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.StateMachines.Resolvers
{
	/// <summary>
	///     Implements a state instance resolver which uses <see cref="ServiceLocator" />.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="IStateResolver" /> for more details.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public sealed class ServiceLocatorStateResolver : IStateResolver
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="ServiceLocatorStateResolver" />.
		/// </summary>
		public ServiceLocatorStateResolver ()
		{
			this.SyncRoot = new object();
		}

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
				return ServiceLocator.GetInstance<IState>(type);
			}
		}

		#endregion
	}
}
