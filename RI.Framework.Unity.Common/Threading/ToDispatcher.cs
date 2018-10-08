﻿using System;

using RI.Framework.ComponentModel;
using RI.Framework.Services.Dispatcher;

using UnityEngine;




namespace RI.Framework.Threading
{
	/// <summary>
	///     Yield instruction to move a task to an <see cref="IDispatcherService" />.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="ThreadMover" /> for more details.
	///     </para>
	/// </remarks>
	public sealed class ToDispatcher : YieldInstruction
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="ToDispatcher" />.
		/// </summary>
		/// <remarks>
		///     <para>
		///         An <see cref="IDispatcherService" /> is used, retrieved using <see cref="ServiceLocator" />.
		///     </para>
		///     <para>
		///         <see cref="DispatcherPriority.Default" /> is used as the priority.
		///     </para>
		/// </remarks>
		public ToDispatcher ()
			: this(ServiceLocator.GetInstance<DispatcherService>() ?? ServiceLocator.GetInstance<IDispatcherService>(), DispatcherPriority.Default)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="ToDispatcher" />.
		/// </summary>
		/// <param name="priority"> The priority to use. </param>
		/// <remarks>
		///     <para>
		///         An <see cref="IDispatcherService" /> is used, retrieved using <see cref="ServiceLocator" />.
		///     </para>
		/// </remarks>
		public ToDispatcher (DispatcherPriority priority)
			: this(ServiceLocator.GetInstance<DispatcherService>() ?? ServiceLocator.GetInstance<IDispatcherService>(), priority)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="ToDispatcher" />.
		/// </summary>
		/// <param name="dispatcherService"> The dispatcher to use. </param>
		/// <remarks>
		///     <para>
		///         <see cref="DispatcherPriority.Default" /> is used as the priority.
		///     </para>
		/// </remarks>
		public ToDispatcher (IDispatcherService dispatcherService)
			: this(dispatcherService, DispatcherPriority.Default)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="ToDispatcher" />.
		/// </summary>
		/// <param name="dispatcherService"> The dispatcher to use. </param>
		/// <param name="priority"> The priority to use. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="dispatcherService" /> is null. </exception>
		public ToDispatcher (IDispatcherService dispatcherService, DispatcherPriority priority)
		{
			if (dispatcherService == null)
			{
				throw new ArgumentNullException(nameof(dispatcherService));
			}

			this.DispatcherService = dispatcherService;
			this.Priority = priority;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the used <see cref="IDispatcherService" />.
		/// </summary>
		/// <value>
		///     The used <see cref="IDispatcherService" /> if used, null otherwise.
		/// </value>
		public IDispatcherService DispatcherService { get; }

		/// <summary>
		///     Gets the used priority.
		/// </summary>
		/// <value>
		///     The used priority if a <see cref="IDispatcherService" /> is used, undefined otherwise.
		/// </value>
		public DispatcherPriority Priority { get; }

		#endregion
	}
}
