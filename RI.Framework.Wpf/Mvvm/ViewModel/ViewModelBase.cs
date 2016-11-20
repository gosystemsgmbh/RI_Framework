using RI.Framework.Services.Logging;
using RI.Framework.Services.Regions;




namespace RI.Framework.Mvvm.ViewModel
{
	/// <summary>
	///     Implements a base class for view models.
	/// </summary>
	public abstract class ViewModelBase : NotificationObject, IViewModel, IRegionElement
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

		/// <inheritdoc cref="IViewModel.Initialize" />
		protected virtual void Initialize ()
		{
		}

		#endregion




		#region Interface: IRegionElement

		/// <inheritdoc />
		public virtual int? SortIndex
		{
			get
			{
				return null;
			}
		}

		/// <inheritdoc />
		public virtual void Activated ()
		{
		}

		/// <inheritdoc />
		public virtual bool CanNavigateFrom ()
		{
			return true;
		}

		/// <inheritdoc />
		public virtual bool CanNavigateTo ()
		{
			return true;
		}

		/// <inheritdoc />
		public virtual void Deactivated ()
		{
		}

		/// <inheritdoc />
		public virtual void NavigatedFrom ()
		{
		}

		/// <inheritdoc />
		public virtual void NavigatedTo ()
		{
		}

		#endregion




		#region Interface: IViewModel

		/// <inheritdoc />
		public bool IsInitialized { get; private set; }

		/// <inheritdoc />
		void IViewModel.Initialize ()
		{
			this.Log(LogLevel.Debug, "Initializing module");

			this.Initialize();

			this.IsInitialized = true;
		}

		#endregion
	}
}
