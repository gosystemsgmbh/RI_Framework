using System;

using RI.Framework.Composition.Model;
using RI.Framework.StateMachines.States;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.StateMachines.Resolvers
{
	/// <summary>
	///     Implements a default state instance resolver suitable for most scenarios.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="DefaultStateResolver" /> simply creates an instance of a requested states type, using <see cref="Activator" />.
	///     </para>
	///     <para>
	///         See <see cref="IStateResolver" /> for more details.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	[Export]
	public sealed class DefaultStateResolver : IStateResolver
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="DefaultStateResolver" />.
		/// </summary>
		public DefaultStateResolver ()
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
				return (IState)Activator.CreateInstance(type);
			}
		}

		#endregion
	}
}
