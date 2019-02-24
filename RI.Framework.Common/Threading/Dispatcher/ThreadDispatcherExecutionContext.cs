using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;

using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Threading.Dispatcher
{
	/// <summary>
	///     Represents the execution context under which a delegate is executed by a dispatcher.
	/// </summary>
	public sealed class ThreadDispatcherExecutionContext : ICloneable<ThreadDispatcherExecutionContext>, ICloneable, IDisposable
	{
		#region Static Methods

		/// <summary>
		///     Captures the current execution context to later use it to execute a delegate.
		/// </summary>
		/// <param name="options"> The use execution options. </param>
		/// <returns>
		///     The captured execution context.
		/// </returns>
		public static ThreadDispatcherExecutionContext Capture (ThreadDispatcherOptions options)
		{
			ThreadDispatcherExecutionContext executionContext = new ThreadDispatcherExecutionContext();

			executionContext.CaptureExecutionContext = (options & ThreadDispatcherOptions.CaptureExecutionContext) == ThreadDispatcherOptions.CaptureExecutionContext;
			executionContext.CaptureSynchronizationContext = (options & ThreadDispatcherOptions.CaptureSynchronizationContext) == ThreadDispatcherOptions.CaptureSynchronizationContext;
			executionContext.CaptureCurrentCulture = (options & ThreadDispatcherOptions.CaptureCurrentCulture) == ThreadDispatcherOptions.CaptureCurrentCulture;
			executionContext.CaptureCurrentUICulture = (options & ThreadDispatcherOptions.CaptureCurrentUICulture) == ThreadDispatcherOptions.CaptureCurrentUICulture;


			executionContext.ExecutionContext = executionContext.CaptureExecutionContext ? ExecutionContext.Capture() : null;
			executionContext.SynchronizationContext = executionContext.CaptureSynchronizationContext ? SynchronizationContext.Current : null;
			executionContext.CurrentCulture = executionContext.CaptureCurrentCulture ? CultureInfo.CurrentCulture : null;
			executionContext.CurrentUICulture = executionContext.CaptureCurrentUICulture ? CultureInfo.CurrentUICulture : null;

			return executionContext;
		}

		#endregion




		#region Instance Constructor/Destructor

		private ThreadDispatcherExecutionContext ()
		{
			this.IsDisposed = false;

			this.CaptureExecutionContext = false;
			this.CaptureSynchronizationContext = false;
			this.CaptureCurrentCulture = false;
			this.CaptureCurrentUICulture = false;

			this.ExecutionContext = null;
			this.SynchronizationContext = null;
			this.CurrentCulture = null;
			this.CurrentUICulture = null;

			this.Options = ThreadDispatcherOptions.None;
			this.Action = null;
			this.Parameters = null;
			this.Result = null;
		}

		/// <summary>
		///     Garbage collects this instance of <see cref="ThreadDispatcherExecutionContext" />.
		/// </summary>
		~ThreadDispatcherExecutionContext ()
		{
			this.Dispose();
		}

		#endregion




		#region Instance Properties/Indexer

		internal bool IsDisposed { get; private set; }

		private Delegate Action { get; set; }
		private CultureInfo CurrentCulture { get; set; }
		private CultureInfo CurrentUICulture { get; set; }
		private ExecutionContext ExecutionContext { get; set; }
		private bool CaptureCurrentCulture { get; set; }
		private bool CaptureCurrentUICulture { get; set; }
		private bool CaptureExecutionContext { get; set; }
		private bool CaptureSynchronizationContext { get; set; }
		private ThreadDispatcherOptions Options { get; set; }
		private object[] Parameters { get; set; }
		private object Result { get; set; }
		private SynchronizationContext SynchronizationContext { get; set; }

		#endregion




		#region Instance Methods

		internal object Run (ThreadDispatcherOptions options, Delegate action, params object[] parameters)
		{
			this.VerifyNotDisposed();

			using (ThreadDispatcherExecutionContext clone = this.Clone())
			{
				return clone.RunInternal(options, action, parameters);
			}
		}

		[SuppressMessage("ReSharper", "EmptyGeneralCatchClause")]
		private void RunCore ()
		{
			bool captureCurrentCulture = ((this.Options & ThreadDispatcherOptions.CaptureCurrentCulture) == ThreadDispatcherOptions.CaptureCurrentCulture) && this.CaptureCurrentCulture;
			bool captureCurrentUICulture = ((this.Options & ThreadDispatcherOptions.CaptureCurrentUICulture) == ThreadDispatcherOptions.CaptureCurrentUICulture) && this.CaptureCurrentUICulture;
			bool captureSynchronizationContext = ((this.Options & ThreadDispatcherOptions.CaptureSynchronizationContext) == ThreadDispatcherOptions.CaptureSynchronizationContext) && this.CaptureSynchronizationContext;

			CultureInfo currentCultureBackup = captureCurrentCulture ? CultureInfo.CurrentCulture : null;
			CultureInfo currentUICultureBackup = captureCurrentUICulture ? CultureInfo.CurrentUICulture : null;
			SynchronizationContext synchronizationContextBackup = captureSynchronizationContext ? SynchronizationContext.Current : null;

			try
			{
				if (captureCurrentCulture)
				{
					CultureInfo.CurrentCulture = this.CurrentCulture;
				}

				if (captureCurrentUICulture)
				{
					CultureInfo.CurrentUICulture = this.CurrentUICulture;
				}

				if (captureSynchronizationContext)
				{
					SynchronizationContext.SetSynchronizationContext(this.SynchronizationContext);
				}

				this.Result = this.Action.DynamicInvoke(this.Parameters);
			}
			finally
			{
				try
				{
					if (captureSynchronizationContext)
					{
						SynchronizationContext.SetSynchronizationContext(synchronizationContextBackup);
					}
				}
				catch
				{
				}

				try
				{
					if (captureCurrentUICulture)
					{
						CultureInfo.CurrentUICulture = currentUICultureBackup;
					}
				}
				catch
				{
				}

				try
				{
					if (captureCurrentCulture)
					{
						CultureInfo.CurrentCulture = currentCultureBackup;
					}
				}
				catch
				{
				}
			}
		}

		private object RunInternal (ThreadDispatcherOptions options, Delegate action, params object[] parameters)
		{
			this.Options = options;
			this.Action = action;
			this.Parameters = parameters;
			this.Result = null;

			bool captureExecutionContext = ((options & ThreadDispatcherOptions.CaptureExecutionContext) == ThreadDispatcherOptions.CaptureExecutionContext) && this.CaptureExecutionContext;
			if (captureExecutionContext)
			{
				ExecutionContext.Run(this.ExecutionContext, state => { ((ThreadDispatcherExecutionContext)state).RunCore(); }, this);
			}
			else
			{
				this.RunCore();
			}

			return this.Result;
		}

		private void VerifyNotDisposed ()
		{
			if (this.IsDisposed)
			{
				throw new ObjectDisposedException(nameof(ThreadDispatcherExecutionContext));
			}
		}

		#endregion




		#region Interface: ICloneable<ThreadDispatcherExecutionContext>

		/// <inheritdoc />
		public ThreadDispatcherExecutionContext Clone ()
		{
			this.VerifyNotDisposed();

			ThreadDispatcherExecutionContext clone = new ThreadDispatcherExecutionContext();

			clone.CaptureExecutionContext = this.CaptureExecutionContext;
			clone.CaptureSynchronizationContext = this.CaptureSynchronizationContext;
			clone.CaptureCurrentCulture = this.CaptureCurrentCulture;
			clone.CaptureCurrentUICulture = this.CaptureCurrentUICulture;

			clone.ExecutionContext = this.ExecutionContext?.CreateCopy();
			clone.SynchronizationContext = this.SynchronizationContext?.CreateCopy() ?? this.SynchronizationContext;
			clone.CurrentCulture = (CultureInfo)this.CurrentCulture?.Clone() ?? this.CurrentCulture;
			clone.CurrentUICulture = (CultureInfo)this.CurrentUICulture?.Clone() ?? this.CurrentUICulture;

			return clone;
		}

		/// <inheritdoc />
		object ICloneable.Clone ()
		{
			return this.Clone();
		}

		#endregion




		#region Interface: IDisposable

		/// <inheritdoc />
		public void Dispose ()
		{
			this.IsDisposed = true;

			this.ExecutionContext?.Dispose();
			this.ExecutionContext = null;

			this.CaptureExecutionContext = false;
			this.CaptureSynchronizationContext = false;
			this.CaptureCurrentCulture = false;
			this.CaptureCurrentUICulture = false;

			this.SynchronizationContext = null;
			this.CurrentCulture = null;
			this.CurrentUICulture = null;

			this.Options = ThreadDispatcherOptions.None;
			this.Action = null;
			this.Parameters = null;
			this.Result = null;
		}

		#endregion
	}
}
