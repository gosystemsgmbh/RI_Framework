﻿using System;

using RI.Framework.Services;
using RI.Framework.Services.Dispatcher;
using RI.Framework.Utilities.ObjectModel;

using UnityEngine;




namespace RI.Framework.Utilities.Threading
{
	/// <summary>
	/// Yield instruction to move a task to an <see cref="IDispatcherService"/> or an <see cref="IThreadDispatcher"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="ThreadMover"/> for more details.
	/// </para>
	/// </remarks>
	public sealed class ToDispatcher : YieldInstruction
	{
		/// <summary>
		/// Creates a new instance of <see cref="ToDispatcher"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// An <see cref="IDispatcherService"/> is used, retrieved using <see cref="ServiceLocator"/> or <see cref="Singleton{IDispatcherService}"/>.
		/// </para>
		/// <para>
		/// <see cref="DispatcherPriority.Default"/> is used as the priority.
		/// </para>
		/// </remarks>
		public ToDispatcher()
			: this(ServiceLocator.GetInstance<IDispatcherService>() ?? Singleton<IDispatcherService>.Instance, DispatcherPriority.Default)
		{
		}

		/// <summary>
		/// Creates a new instance of <see cref="ToDispatcher"/>.
		/// </summary>
		/// <param name="priority">The priority to use.</param>
		/// <remarks>
		/// <para>
		/// An <see cref="IDispatcherService"/> is used, retrieved using <see cref="ServiceLocator"/> or <see cref="Singleton{IDispatcherService}"/>.
		/// </para>
		/// </remarks>
		public ToDispatcher(DispatcherPriority priority)
			: this(ServiceLocator.GetInstance<IDispatcherService>() ?? Singleton<IDispatcherService>.Instance, priority)
		{
		}

		/// <summary>
		/// Creates a new instance of <see cref="ToDispatcher"/>.
		/// </summary>
		/// <param name="dispatcherService">The dispatcher to use.</param>
		/// <remarks>
		/// <para>
		/// <see cref="DispatcherPriority.Default"/> is used as the priority.
		/// </para>
		/// </remarks>
		public ToDispatcher(IDispatcherService dispatcherService)
			: this(dispatcherService, DispatcherPriority.Default)
		{
		}

		/// <summary>
		/// Creates a new instance of <see cref="ToDispatcher"/>.
		/// </summary>
		/// <param name="dispatcherService">The dispatcher to use.</param>
		/// <param name="priority">The priority to use.</param>
		/// <exception cref="ArgumentNullException"><paramref name="dispatcherService"/> is null.</exception>
		public ToDispatcher (IDispatcherService dispatcherService, DispatcherPriority priority)
		{
			if (dispatcherService == null)
			{
				throw new ArgumentNullException(nameof(dispatcherService));
			}

			this.DispatcherService = dispatcherService;
			this.Priority = priority;
			this.ThreadDispatcher = null;
		}

		/// <summary>
		/// Creates a new instance of <see cref="ToDispatcher"/>.
		/// </summary>
		/// <param name="threadDispatcher">The dispatcher to use.</param>
		/// <exception cref="ArgumentNullException"><paramref name="threadDispatcher"/> is null.</exception>
		public ToDispatcher(IThreadDispatcher threadDispatcher)
		{
			if (threadDispatcher == null)
			{
				throw new ArgumentNullException(nameof(threadDispatcher));
			}

			this.DispatcherService = null;
			this.ThreadDispatcher = threadDispatcher;
		}

		/// <summary>
		/// Gets the used priority.
		/// </summary>
		/// <value>
		/// The used priority if a <see cref="IDispatcherService"/> is used, undefined otherwise.
		/// </value>
		public DispatcherPriority Priority { get; }

		/// <summary>
		/// Gets the used <see cref="IDispatcherService"/>.
		/// </summary>
		/// <value>
		/// The used <see cref="IDispatcherService"/> if used, null otherwise.
		/// </value>
		public IDispatcherService DispatcherService { get; }

		/// <summary>
		/// Gets the used <see cref="IThreadDispatcher"/>.
		/// </summary>
		/// <value>
		/// The used <see cref="IThreadDispatcher"/> if used, null otherwise.
		/// </value>
		public IThreadDispatcher ThreadDispatcher { get; }
	}
}