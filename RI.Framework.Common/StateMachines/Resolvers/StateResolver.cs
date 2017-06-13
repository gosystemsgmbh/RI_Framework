using System;

using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.StateMachines.Resolvers
{
	/// <summary>
	///     Implements a default state instance resolver suitable for most scenarios.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="StateResolver" /> simply creates an instance of a requested states type, using <see cref="Activator" />.
	///     </para>
	///     <para>
	///         See <see cref="IStateResolver" /> for more details.
	///     </para>
	/// </remarks>
	public sealed class StateResolver : IStateResolver
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="StateResolver" />.
		/// </summary>
		public StateResolver ()
		{
			this.SyncRoot = new object();
		}

		#endregion




		#region Interface: IStateResolver

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		public object SyncRoot { get; private set; }

		/// <inheritdoc />
		public IState ResolveState (Type type)
		{
			lock (this.SyncRoot)
			{
				return (IState)Activator.CreateInstance(type);
			}
		}

		#endregion
	}
}
