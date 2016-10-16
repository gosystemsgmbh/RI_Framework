using System.ComponentModel;
using System.Windows;
using System.Windows.Input;




namespace RI.Framework.Mvvm
{
	/// <summary>
	///     Provides attached properties to bind specific events of WPF objects to commands.
	/// </summary>
	/// <remarks>
	///     <para>
	///         The attached properties provided by this class can be used to bind various events to <see cref="ICommand" />s, usually provided by the view model.
	///     </para>
	/// </remarks>
	public static class EventBinder
	{
		#region Constants

		/// <summary>
		///     Binds the <see cref="Window.Closing" /> event to a command.
		/// </summary>
		public static readonly DependencyProperty ClosingEventProperty = DependencyProperty.RegisterAttached("ClosingEvent", typeof(ICommand), typeof(EventBinder), new FrameworkPropertyMetadata(EventBinder.OnClosingEventChanged));

		#endregion




		#region Static Properties/Indexer

		private static CancelEventHandler ClosingEventHandler { get; set; } = (s, e) =>
		{
			ICommand command = EventBinder.GetClosingEvent(s as DependencyObject);
			if (command != null)
			{
				if (command.CanExecute(e))
				{
					command.Execute(e);
				}
			}
		};

		#endregion




		#region Static Methods

		/// <summary>
		///     Gets the command bound to the <see cref="Window.Closing" /> event.
		/// </summary>
		/// <param name="obj"> The container. </param>
		/// <returns>
		///     The command bound to the <see cref="Window.Closing" /> event.
		/// </returns>
		/// <remarks>
		///     <note type="note">
		///         This method is for supporting the XAML designer and not intended to be used by your code.
		///     </note>
		/// </remarks>
		public static ICommand GetClosingEvent (DependencyObject obj)
		{
			return obj?.GetValue(EventBinder.ClosingEventProperty) as ICommand;
		}

		/// <summary>
		///     Sets the command bound to the <see cref="Window.Closing" /> event.
		/// </summary>
		/// <param name="obj"> The container. </param>
		/// <param name="value"> The command to bind to the <see cref="Window.Closing" /> event. </param>
		/// <remarks>
		///     <note type="note">
		///         This method is for supporting the XAML designer and not intended to be used by your code.
		///     </note>
		/// </remarks>
		public static void SetClosingEvent (DependencyObject obj, ICommand value)
		{
			obj?.SetValue(EventBinder.ClosingEventProperty, value);
		}

		private static void OnClosingEventChanged (DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			ICommand oldCommand = args.OldValue as ICommand;
			ICommand newCommand = args.NewValue as ICommand;
			Window window = obj as Window;

			if (window == null)
			{
				return;
			}

			if (object.ReferenceEquals(oldCommand, newCommand))
			{
				return;
			}

			if (oldCommand != null)
			{
				window.Closing -= EventBinder.ClosingEventHandler;
			}

			if (newCommand != null)
			{
				window.Closing += EventBinder.ClosingEventHandler;
			}
		}

		#endregion
	}
}
