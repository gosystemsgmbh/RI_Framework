using System;

using RI.Framework.Composition;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.StateMachines.Resolvers
{
	/// <summary>
	///     Implements a state instance resolver which uses a <see cref="CompositionContainer"/>.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="IStateResolver" /> for more details.
	///     </para>
	/// </remarks>
	public sealed class CompositionContainerStateResolver : IStateResolver
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="CompositionContainerStateResolver" />.
		/// </summary>
		/// <param name="container"> </param>
		/// <exception cref="ArgumentNullException"> <paramref name="container" /> is null. </exception>
		public CompositionContainerStateResolver(CompositionContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException(nameof(container));
			}

			this.Container = container;

			this.SyncRoot = new object();
		}

		#endregion




		#region Instance Properties/Indexer

		private CompositionContainer Container { get; set; }

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		public object SyncRoot { get; private set; }

		#endregion




		#region Interface: IStateResolver

		/// <inheritdoc />
		public IState ResolveState (Type type)
		{
			lock (this.SyncRoot)
			{
				return this.Container.GetExport<IState>(type);
			}
		}

		#endregion
	}
}
