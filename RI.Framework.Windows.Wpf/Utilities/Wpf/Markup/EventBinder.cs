using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;




namespace RI.Framework.Utilities.Wpf.Markup
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

		/// <summary>
		///     Binds the <see cref="Control.MouseDoubleClick" /> event to a command.
		/// </summary>
		public static readonly DependencyProperty MouseDoubleClickEventProperty = DependencyProperty.RegisterAttached("MouseDoubleClickEvent", typeof(ICommand), typeof(EventBinder), new FrameworkPropertyMetadata(EventBinder.OnMouseDoubleClickEventChanged));

		/// <summary>
		///     Binds the <see cref="UIElement.MouseDown" /> event to a command.
		/// </summary>
		public static readonly DependencyProperty MouseDownEventProperty = DependencyProperty.RegisterAttached("MouseDownEvent", typeof(ICommand), typeof(EventBinder), new FrameworkPropertyMetadata(EventBinder.OnMouseDownEventChanged));

		/// <summary>
		///     Binds the <see cref="UIElement.PreviewKeyDown" /> event to a command.
		/// </summary>
		public static readonly DependencyProperty PreviewKeyDownEventProperty = DependencyProperty.RegisterAttached("PreviewKeyDownEvent", typeof(ICommand), typeof(EventBinder), new FrameworkPropertyMetadata(EventBinder.OnPreviewKeyDownEventChanged));

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

		private static MouseButtonEventHandler MouseDoubleClickEventHandler { get; set; } = (s, e) =>
		{
			ICommand command = EventBinder.GetMouseDoubleClickEvent(s as DependencyObject);
			if (command != null)
			{
				if (command.CanExecute(e))
				{
					command.Execute(e);
				}
			}
		};

		private static MouseButtonEventHandler MouseDownEventHandler { get; set; } = (s, e) =>
		{
			ICommand command = EventBinder.GetMouseDownEvent(s as DependencyObject);
			if (command != null)
			{
				if (command.CanExecute(e))
				{
					command.Execute(e);
				}
			}
		};

		private static KeyEventHandler PreviewKeyDownEventHandler { get; set; } = (s, e) =>
		{
			ICommand command = EventBinder.GetPreviewKeyDownEvent(s as DependencyObject);
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
		///     Gets the command bound to the <see cref="Control.MouseDoubleClick" /> event.
		/// </summary>
		/// <param name="obj"> The container. </param>
		/// <returns>
		///     The command bound to the <see cref="Control.MouseDoubleClick" /> event.
		/// </returns>
		/// <remarks>
		///     <note type="note">
		///         This method is for supporting the XAML designer and not intended to be used by your code.
		///     </note>
		/// </remarks>
		public static ICommand GetMouseDoubleClickEvent (DependencyObject obj)
		{
			return obj?.GetValue(EventBinder.MouseDoubleClickEventProperty) as ICommand;
		}

		/// <summary>
		///     Gets the command bound to the <see cref="UIElement.MouseDown" /> event.
		/// </summary>
		/// <param name="obj"> The container. </param>
		/// <returns>
		///     The command bound to the <see cref="UIElement.MouseDown" /> event.
		/// </returns>
		/// <remarks>
		///     <note type="note">
		///         This method is for supporting the XAML designer and not intended to be used by your code.
		///     </note>
		/// </remarks>
		public static ICommand GetMouseDownEvent (DependencyObject obj)
		{
			return obj?.GetValue(EventBinder.MouseDownEventProperty) as ICommand;
		}

		/// <summary>
		///     Gets the command bound to the <see cref="UIElement.PreviewKeyDown" /> event.
		/// </summary>
		/// <param name="obj"> The container. </param>
		/// <returns>
		///     The command bound to the <see cref="UIElement.PreviewKeyDown" /> event.
		/// </returns>
		/// <remarks>
		///     <note type="note">
		///         This method is for supporting the XAML designer and not intended to be used by your code.
		///     </note>
		/// </remarks>
		public static ICommand GetPreviewKeyDownEvent (DependencyObject obj)
		{
			return obj?.GetValue(EventBinder.PreviewKeyDownEventProperty) as ICommand;
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

		/// <summary>
		///     Sets the command bound to the <see cref="Control.MouseDoubleClick" /> event.
		/// </summary>
		/// <param name="obj"> The container. </param>
		/// <param name="value"> The command to bind to the <see cref="Control.MouseDoubleClick" /> event. </param>
		/// <remarks>
		///     <note type="note">
		///         This method is for supporting the XAML designer and not intended to be used by your code.
		///     </note>
		/// </remarks>
		public static void SetMouseDoubleClickEvent (DependencyObject obj, ICommand value)
		{
			obj?.SetValue(EventBinder.MouseDoubleClickEventProperty, value);
		}

		/// <summary>
		///     Sets the command bound to the <see cref="UIElement.MouseDown" /> event.
		/// </summary>
		/// <param name="obj"> The container. </param>
		/// <param name="value"> The command to bind to the <see cref="UIElement.MouseDown" /> event. </param>
		/// <remarks>
		///     <note type="note">
		///         This method is for supporting the XAML designer and not intended to be used by your code.
		///     </note>
		/// </remarks>
		public static void SetMouseDownEvent (DependencyObject obj, ICommand value)
		{
			obj?.SetValue(EventBinder.MouseDownEventProperty, value);
		}

		/// <summary>
		///     Sets the command bound to the <see cref="UIElement.PreviewKeyDown" /> event.
		/// </summary>
		/// <param name="obj"> The container. </param>
		/// <param name="value"> The command to bind to the <see cref="UIElement.PreviewKeyDown" /> event. </param>
		/// <remarks>
		///     <note type="note">
		///         This method is for supporting the XAML designer and not intended to be used by your code.
		///     </note>
		/// </remarks>
		public static void SetPreviewKeyDownEvent (DependencyObject obj, ICommand value)
		{
			obj?.SetValue(EventBinder.PreviewKeyDownEventProperty, value);
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

		private static void OnMouseDoubleClickEventChanged (DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			ICommand oldCommand = args.OldValue as ICommand;
			ICommand newCommand = args.NewValue as ICommand;
			Control control = obj as Control;

			if (control == null)
			{
				return;
			}

			if (object.ReferenceEquals(oldCommand, newCommand))
			{
				return;
			}

			if (oldCommand != null)
			{
				control.MouseDoubleClick -= EventBinder.MouseDoubleClickEventHandler;
			}

			if (newCommand != null)
			{
				control.MouseDoubleClick += EventBinder.MouseDoubleClickEventHandler;
			}
		}

		private static void OnMouseDownEventChanged (DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			ICommand oldCommand = args.OldValue as ICommand;
			ICommand newCommand = args.NewValue as ICommand;
			UIElement control = obj as UIElement;

			if (control == null)
			{
				return;
			}

			if (object.ReferenceEquals(oldCommand, newCommand))
			{
				return;
			}

			if (oldCommand != null)
			{
				control.MouseDown -= EventBinder.MouseDownEventHandler;
			}

			if (newCommand != null)
			{
				control.MouseDown += EventBinder.MouseDownEventHandler;
			}
		}

		private static void OnPreviewKeyDownEventChanged (DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			ICommand oldCommand = args.OldValue as ICommand;
			ICommand newCommand = args.NewValue as ICommand;
			UIElement uiElement = obj as UIElement;

			if (uiElement == null)
			{
				return;
			}

			if (object.ReferenceEquals(oldCommand, newCommand))
			{
				return;
			}

			if (oldCommand != null)
			{
				uiElement.PreviewKeyDown -= EventBinder.PreviewKeyDownEventHandler;
			}

			if (newCommand != null)
			{
				uiElement.PreviewKeyDown += EventBinder.PreviewKeyDownEventHandler;
			}
		}

		#endregion
	}
}
