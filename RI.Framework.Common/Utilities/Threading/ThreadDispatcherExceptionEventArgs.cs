﻿using System;

namespace RI.Framework.Utilities.Threading
{
	/// <summary>
	/// Event arguments for the <see cref="ThreadDispatcher" />.<see cref="ThreadDispatcher.Exception" /> and <see cref="HeavyThreadDispatcher" />.<see cref="HeavyThreadDispatcher.Exception" /> event.
	/// </summary>
	public sealed class ThreadDispatcherExceptionEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the exception.
		/// </summary>
		/// <value>
		/// The exception.
		/// </value>
		public Exception Exception { get; }

		/// <summary>
		/// Gets whether the thread is able to continue or not after the exception was handled.
		/// </summary>
		/// <value>
		/// true if the thread is able to continue, false otherwise.
		/// </value>
		public bool CanContinue { get; }

		/// <summary>
		/// Creates a new instance of <see cref="ThreadDispatcherExceptionEventArgs"/>.
		/// </summary>
		/// <param name="exception">The exception.</param>
		/// <param name="canContinue">Indicates whether the thread is able to continue or not after the exception was handled.</param>
		public ThreadDispatcherExceptionEventArgs (Exception exception, bool canContinue)
		{
			this.Exception = exception;
			this.CanContinue = canContinue;
		}
	}
}
