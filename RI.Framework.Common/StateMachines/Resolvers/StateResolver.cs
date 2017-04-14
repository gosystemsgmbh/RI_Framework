using System;

using RI.Framework.Composition;
using RI.Framework.Services;




namespace RI.Framework.StateMachines
{
	/// <summary>
	///     Implements a default state instance resolver suitable for most scenarios.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="StateResolver" /> internally uses <see cref="ServiceLocator" /> or a specified <see cref="CompositionContainer" /> to resolve the state instances, depending on the used constructor.
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
		/// <remarks>
		///     <para>
		///         <see cref="StateResolver" /> instances created with this constructor use <see cref="ServiceLocator" />.
		///     </para>
		/// </remarks>
		public StateResolver ()
		{
			this.Container = null;
		}

		/// <summary>
		///     Creates a new instance of <see cref="StateResolver" />.
		/// </summary>
		/// <param name="container"> </param>
		/// <remarks>
		///     <para>
		///         <see cref="StateResolver" /> instances created with this constructor use the <see cref="CompositionContainer" /> specified by <paramref name="container" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="container" /> is null. </exception>
		public StateResolver (CompositionContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException(nameof(container));
			}

			this.Container = container;
		}

		#endregion




		#region Instance Properties/Indexer

		private CompositionContainer Container { get; set; }

		#endregion




		#region Interface: IStateResolver

		/// <inheritdoc />
		public IState ResolveState (Type type)
		{
			if (this.Container == null)
			{
				return ServiceLocator.GetInstance(type) as IState;
			}

			return this.Container.GetExport<IState>(type);
		}

		#endregion
	}
}
