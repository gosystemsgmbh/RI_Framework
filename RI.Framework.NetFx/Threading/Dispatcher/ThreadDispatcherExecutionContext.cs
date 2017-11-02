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

			executionContext.FlowExecutionContext = (options & ThreadDispatcherOptions.FlowExecutionContext) == ThreadDispatcherOptions.FlowExecutionContext;
			executionContext.FlowSynchronizationContext = (options & ThreadDispatcherOptions.FlowSynchronizationContext) == ThreadDispatcherOptions.FlowSynchronizationContext;
			executionContext.FlowCurrentCulture = (options & ThreadDispatcherOptions.FlowCurrentCulture) == ThreadDispatcherOptions.FlowCurrentCulture;
			executionContext.FlowCurrentUICulture = (options & ThreadDispatcherOptions.FlowCurrentUICulture) == ThreadDispatcherOptions.FlowCurrentUICulture;


			executionContext.ExecutionContext = executionContext.FlowExecutionContext ? ExecutionContext.Capture() : null;
			executionContext.SynchronizationContext = executionContext.FlowSynchronizationContext ? SynchronizationContext.Current : null;
			executionContext.CurrentCulture = executionContext.FlowCurrentCulture ? CultureInfo.CurrentCulture : null;
			executionContext.CurrentUICulture = executionContext.FlowCurrentUICulture ? CultureInfo.CurrentUICulture : null;

			return executionContext;
		}

		#endregion




		#region Instance Constructor/Destructor

		private ThreadDispatcherExecutionContext ()
		{
			this.IsDisposed = false;

			this.FlowExecutionContext = false;
			this.FlowSynchronizationContext = false;
			this.FlowCurrentCulture = false;
			this.FlowCurrentUICulture = false;

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

		private Delegate Action { get; set; }
		private CultureInfo CurrentCulture { get; set; }
		private CultureInfo CurrentUICulture { get; set; }
		private ExecutionContext ExecutionContext { get; set; }
		private bool FlowCurrentCulture { get; set; }
		private bool FlowCurrentUICulture { get; set; }
		private bool FlowExecutionContext { get; set; }
		private bool FlowSynchronizationContext { get; set; }
		internal bool IsDisposed { get; private set; }
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
			bool flowCurrentCulture = ((this.Options & ThreadDispatcherOptions.FlowCurrentCulture) == ThreadDispatcherOptions.FlowCurrentCulture) && this.FlowCurrentCulture;
			bool flowCurrentUICulture = ((this.Options & ThreadDispatcherOptions.FlowCurrentUICulture) == ThreadDispatcherOptions.FlowCurrentUICulture) && this.FlowCurrentUICulture;
			bool flowSynchronizationContext = ((this.Options & ThreadDispatcherOptions.FlowSynchronizationContext) == ThreadDispatcherOptions.FlowSynchronizationContext) && this.FlowSynchronizationContext;

			CultureInfo currentCultureBackup = flowCurrentCulture ? CultureInfo.CurrentCulture : null;
			CultureInfo currentUICultureBackup = flowCurrentUICulture ? CultureInfo.CurrentUICulture : null;
			SynchronizationContext synchronizationContextBackup = flowSynchronizationContext ? SynchronizationContext.Current : null;

			try
			{
				if (flowCurrentCulture)
				{
					CultureInfo.CurrentCulture = this.CurrentCulture;
				}

				if (flowCurrentUICulture)
				{
					CultureInfo.CurrentUICulture = this.CurrentUICulture;
				}

				if (flowSynchronizationContext)
				{
					SynchronizationContext.SetSynchronizationContext(this.SynchronizationContext);
				}

				this.Result = this.Action.DynamicInvoke(this.Parameters);
			}
			finally
			{
				try
				{
					if (flowSynchronizationContext)
					{
						SynchronizationContext.SetSynchronizationContext(synchronizationContextBackup);
					}
				}
				catch
				{
				}

				try
				{
					if (flowCurrentUICulture)
					{
						CultureInfo.CurrentUICulture = currentUICultureBackup;
					}
				}
				catch
				{
				}

				try
				{
					if (flowCurrentCulture)
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

			bool flowExecutionContext = ((options & ThreadDispatcherOptions.FlowExecutionContext) == ThreadDispatcherOptions.FlowExecutionContext) && this.FlowExecutionContext;
			if (flowExecutionContext)
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

			clone.FlowExecutionContext = this.FlowExecutionContext;
			clone.FlowSynchronizationContext = this.FlowSynchronizationContext;
			clone.FlowCurrentCulture = this.FlowCurrentCulture;
			clone.FlowCurrentUICulture = this.FlowCurrentUICulture;

			clone.ExecutionContext = this.ExecutionContext?.CreateCopy();
			clone.SynchronizationContext = this.SynchronizationContext?.CreateCopy();
			clone.CurrentCulture = (CultureInfo)this.CurrentCulture?.Clone();
			clone.CurrentUICulture = (CultureInfo)this.CurrentUICulture?.Clone();

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

			this.FlowExecutionContext = false;
			this.FlowSynchronizationContext = false;
			this.FlowCurrentCulture = false;
			this.FlowCurrentUICulture = false;

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
