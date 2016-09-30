using RI.Framework.Services.Logging;




namespace RI.Framework.Services.Modularization
{
	/// <summary>
	///     Implements a base class which can be used for module implementation.
	/// </summary>
	/// <para>
	///     See <see cref="IModule" /> for more details.
	/// </para>
	public abstract class Module : IModule
	{
		#region Instance Methods

		/// <summary>
		///     Logs a message.
		/// </summary>
		/// <param name="severity"> The severity of the message. </param>
		/// <param name="format"> The message. </param>
		/// <param name="args"> The arguments which will be expanded into the message (comparable to <see cref="string.Format(string, object[])" />). </param>
		/// <remarks>
		///     <para>
		///         <see cref="ILogService" /> is used, obtained through <see cref="LogLocator" />.
		///         If no <see cref="ILogService" /> is available, no logging is performed.
		///     </para>
		/// </remarks>
		protected void Log (LogLevel severity, string format, params object[] args)
		{
			LogLocator.Log(severity, this.GetType().Name, format, args);
		}

		#endregion




		#region Virtuals

		/// <inheritdoc cref="IModule.Initialize" />
		protected virtual void Initialize ()
		{
		}

		/// <inheritdoc cref="IModule.Unload" />
		protected virtual void Unload ()
		{
		}

		#endregion




		#region Interface: IModule

		/// <inheritdoc />
		public bool IsInitialized { get; private set; }

		/// <inheritdoc />
		void IModule.Initialize ()
		{
			this.Log(LogLevel.Debug, "Initializing module");

			this.Initialize();

			this.IsInitialized = true;
		}

		/// <inheritdoc />
		void IModule.Unload ()
		{
			this.Log(LogLevel.Debug, "Unloading module");

			this.Unload();

			this.IsInitialized = false;
		}

		#endregion
	}
}
