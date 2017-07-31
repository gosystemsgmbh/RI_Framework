

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


		/// <summary>
		///     Binds the <see cref="Window.Closing" /> event to a command.
		/// </summary>
		public static readonly DependencyProperty ClosingEventProperty = DependencyProperty.RegisterAttached("ClosingEvent", typeof(ICommand), typeof(EventBinder), new FrameworkPropertyMetadata(EventBinder.OnWindowClosingEventChanged));

		private static CancelEventHandler WindowClosingEventHandler { get; set; } = (s, e) =>
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

		private static void OnWindowClosingEventChanged (DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			ICommand oldCommand = args.OldValue as ICommand;
			ICommand newCommand = args.NewValue as ICommand;
			Window control = obj as Window;

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
				control.Closing -= EventBinder.WindowClosingEventHandler;
			}

			if (newCommand != null)
			{
				control.Closing += EventBinder.WindowClosingEventHandler;
			}
		}


		/// <summary>
		///     Binds the <see cref="Control.PreviewMouseDoubleClick" /> event to a command.
		/// </summary>
		public static readonly DependencyProperty PreviewMouseDoubleClickEventProperty = DependencyProperty.RegisterAttached("PreviewMouseDoubleClickEvent", typeof(ICommand), typeof(EventBinder), new FrameworkPropertyMetadata(EventBinder.OnControlPreviewMouseDoubleClickEventChanged));

		private static MouseButtonEventHandler ControlPreviewMouseDoubleClickEventHandler { get; set; } = (s, e) =>
		{
			ICommand command = EventBinder.GetPreviewMouseDoubleClickEvent(s as DependencyObject);
			if (command != null)
			{
				if (command.CanExecute(e))
				{
					command.Execute(e);
				}
			}
		};

		/// <summary>
		///     Gets the command bound to the <see cref="Control.PreviewMouseDoubleClick" /> event.
		/// </summary>
		/// <param name="obj"> The container. </param>
		/// <returns>
		///     The command bound to the <see cref="Control.PreviewMouseDoubleClick" /> event.
		/// </returns>
		/// <remarks>
		///     <note type="note">
		///         This method is for supporting the XAML designer and not intended to be used by your code.
		///     </note>
		/// </remarks>
		public static ICommand GetPreviewMouseDoubleClickEvent (DependencyObject obj)
		{
			return obj?.GetValue(EventBinder.PreviewMouseDoubleClickEventProperty) as ICommand;
		}

		/// <summary>
		///     Sets the command bound to the <see cref="Control.PreviewMouseDoubleClick" /> event.
		/// </summary>
		/// <param name="obj"> The container. </param>
		/// <param name="value"> The command to bind to the <see cref="Control.PreviewMouseDoubleClick" /> event. </param>
		/// <remarks>
		///     <note type="note">
		///         This method is for supporting the XAML designer and not intended to be used by your code.
		///     </note>
		/// </remarks>
		public static void SetPreviewMouseDoubleClickEvent (DependencyObject obj, ICommand value)
		{
			obj?.SetValue(EventBinder.PreviewMouseDoubleClickEventProperty, value);
		}

		private static void OnControlPreviewMouseDoubleClickEventChanged (DependencyObject obj, DependencyPropertyChangedEventArgs args)
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
				control.PreviewMouseDoubleClick -= EventBinder.ControlPreviewMouseDoubleClickEventHandler;
			}

			if (newCommand != null)
			{
				control.PreviewMouseDoubleClick += EventBinder.ControlPreviewMouseDoubleClickEventHandler;
			}
		}


		/// <summary>
		///     Binds the <see cref="Control.MouseDoubleClick" /> event to a command.
		/// </summary>
		public static readonly DependencyProperty MouseDoubleClickEventProperty = DependencyProperty.RegisterAttached("MouseDoubleClickEvent", typeof(ICommand), typeof(EventBinder), new FrameworkPropertyMetadata(EventBinder.OnControlMouseDoubleClickEventChanged));

		private static MouseButtonEventHandler ControlMouseDoubleClickEventHandler { get; set; } = (s, e) =>
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

		private static void OnControlMouseDoubleClickEventChanged (DependencyObject obj, DependencyPropertyChangedEventArgs args)
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
				control.MouseDoubleClick -= EventBinder.ControlMouseDoubleClickEventHandler;
			}

			if (newCommand != null)
			{
				control.MouseDoubleClick += EventBinder.ControlMouseDoubleClickEventHandler;
			}
		}


		/// <summary>
		///     Binds the <see cref="UIElement.PreviewMouseDown" /> event to a command.
		/// </summary>
		public static readonly DependencyProperty PreviewMouseDownEventProperty = DependencyProperty.RegisterAttached("PreviewMouseDownEvent", typeof(ICommand), typeof(EventBinder), new FrameworkPropertyMetadata(EventBinder.OnUIElementPreviewMouseDownEventChanged));

		private static MouseButtonEventHandler UIElementPreviewMouseDownEventHandler { get; set; } = (s, e) =>
		{
			ICommand command = EventBinder.GetPreviewMouseDownEvent(s as DependencyObject);
			if (command != null)
			{
				if (command.CanExecute(e))
				{
					command.Execute(e);
				}
			}
		};

		/// <summary>
		///     Gets the command bound to the <see cref="UIElement.PreviewMouseDown" /> event.
		/// </summary>
		/// <param name="obj"> The container. </param>
		/// <returns>
		///     The command bound to the <see cref="UIElement.PreviewMouseDown" /> event.
		/// </returns>
		/// <remarks>
		///     <note type="note">
		///         This method is for supporting the XAML designer and not intended to be used by your code.
		///     </note>
		/// </remarks>
		public static ICommand GetPreviewMouseDownEvent (DependencyObject obj)
		{
			return obj?.GetValue(EventBinder.PreviewMouseDownEventProperty) as ICommand;
		}

		/// <summary>
		///     Sets the command bound to the <see cref="UIElement.PreviewMouseDown" /> event.
		/// </summary>
		/// <param name="obj"> The container. </param>
		/// <param name="value"> The command to bind to the <see cref="UIElement.PreviewMouseDown" /> event. </param>
		/// <remarks>
		///     <note type="note">
		///         This method is for supporting the XAML designer and not intended to be used by your code.
		///     </note>
		/// </remarks>
		public static void SetPreviewMouseDownEvent (DependencyObject obj, ICommand value)
		{
			obj?.SetValue(EventBinder.PreviewMouseDownEventProperty, value);
		}

		private static void OnUIElementPreviewMouseDownEventChanged (DependencyObject obj, DependencyPropertyChangedEventArgs args)
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
				control.PreviewMouseDown -= EventBinder.UIElementPreviewMouseDownEventHandler;
			}

			if (newCommand != null)
			{
				control.PreviewMouseDown += EventBinder.UIElementPreviewMouseDownEventHandler;
			}
		}


		/// <summary>
		///     Binds the <see cref="UIElement.PreviewMouseUp" /> event to a command.
		/// </summary>
		public static readonly DependencyProperty PreviewMouseUpEventProperty = DependencyProperty.RegisterAttached("PreviewMouseUpEvent", typeof(ICommand), typeof(EventBinder), new FrameworkPropertyMetadata(EventBinder.OnUIElementPreviewMouseUpEventChanged));

		private static MouseButtonEventHandler UIElementPreviewMouseUpEventHandler { get; set; } = (s, e) =>
		{
			ICommand command = EventBinder.GetPreviewMouseUpEvent(s as DependencyObject);
			if (command != null)
			{
				if (command.CanExecute(e))
				{
					command.Execute(e);
				}
			}
		};

		/// <summary>
		///     Gets the command bound to the <see cref="UIElement.PreviewMouseUp" /> event.
		/// </summary>
		/// <param name="obj"> The container. </param>
		/// <returns>
		///     The command bound to the <see cref="UIElement.PreviewMouseUp" /> event.
		/// </returns>
		/// <remarks>
		///     <note type="note">
		///         This method is for supporting the XAML designer and not intended to be used by your code.
		///     </note>
		/// </remarks>
		public static ICommand GetPreviewMouseUpEvent (DependencyObject obj)
		{
			return obj?.GetValue(EventBinder.PreviewMouseUpEventProperty) as ICommand;
		}

		/// <summary>
		///     Sets the command bound to the <see cref="UIElement.PreviewMouseUp" /> event.
		/// </summary>
		/// <param name="obj"> The container. </param>
		/// <param name="value"> The command to bind to the <see cref="UIElement.PreviewMouseUp" /> event. </param>
		/// <remarks>
		///     <note type="note">
		///         This method is for supporting the XAML designer and not intended to be used by your code.
		///     </note>
		/// </remarks>
		public static void SetPreviewMouseUpEvent (DependencyObject obj, ICommand value)
		{
			obj?.SetValue(EventBinder.PreviewMouseUpEventProperty, value);
		}

		private static void OnUIElementPreviewMouseUpEventChanged (DependencyObject obj, DependencyPropertyChangedEventArgs args)
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
				control.PreviewMouseUp -= EventBinder.UIElementPreviewMouseUpEventHandler;
			}

			if (newCommand != null)
			{
				control.PreviewMouseUp += EventBinder.UIElementPreviewMouseUpEventHandler;
			}
		}


		/// <summary>
		///     Binds the <see cref="UIElement.MouseDown" /> event to a command.
		/// </summary>
		public static readonly DependencyProperty MouseDownEventProperty = DependencyProperty.RegisterAttached("MouseDownEvent", typeof(ICommand), typeof(EventBinder), new FrameworkPropertyMetadata(EventBinder.OnUIElementMouseDownEventChanged));

		private static MouseButtonEventHandler UIElementMouseDownEventHandler { get; set; } = (s, e) =>
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

		private static void OnUIElementMouseDownEventChanged (DependencyObject obj, DependencyPropertyChangedEventArgs args)
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
				control.MouseDown -= EventBinder.UIElementMouseDownEventHandler;
			}

			if (newCommand != null)
			{
				control.MouseDown += EventBinder.UIElementMouseDownEventHandler;
			}
		}


		/// <summary>
		///     Binds the <see cref="UIElement.MouseUp" /> event to a command.
		/// </summary>
		public static readonly DependencyProperty MouseUpEventProperty = DependencyProperty.RegisterAttached("MouseUpEvent", typeof(ICommand), typeof(EventBinder), new FrameworkPropertyMetadata(EventBinder.OnUIElementMouseUpEventChanged));

		private static MouseButtonEventHandler UIElementMouseUpEventHandler { get; set; } = (s, e) =>
		{
			ICommand command = EventBinder.GetMouseUpEvent(s as DependencyObject);
			if (command != null)
			{
				if (command.CanExecute(e))
				{
					command.Execute(e);
				}
			}
		};

		/// <summary>
		///     Gets the command bound to the <see cref="UIElement.MouseUp" /> event.
		/// </summary>
		/// <param name="obj"> The container. </param>
		/// <returns>
		///     The command bound to the <see cref="UIElement.MouseUp" /> event.
		/// </returns>
		/// <remarks>
		///     <note type="note">
		///         This method is for supporting the XAML designer and not intended to be used by your code.
		///     </note>
		/// </remarks>
		public static ICommand GetMouseUpEvent (DependencyObject obj)
		{
			return obj?.GetValue(EventBinder.MouseUpEventProperty) as ICommand;
		}

		/// <summary>
		///     Sets the command bound to the <see cref="UIElement.MouseUp" /> event.
		/// </summary>
		/// <param name="obj"> The container. </param>
		/// <param name="value"> The command to bind to the <see cref="UIElement.MouseUp" /> event. </param>
		/// <remarks>
		///     <note type="note">
		///         This method is for supporting the XAML designer and not intended to be used by your code.
		///     </note>
		/// </remarks>
		public static void SetMouseUpEvent (DependencyObject obj, ICommand value)
		{
			obj?.SetValue(EventBinder.MouseUpEventProperty, value);
		}

		private static void OnUIElementMouseUpEventChanged (DependencyObject obj, DependencyPropertyChangedEventArgs args)
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
				control.MouseUp -= EventBinder.UIElementMouseUpEventHandler;
			}

			if (newCommand != null)
			{
				control.MouseUp += EventBinder.UIElementMouseUpEventHandler;
			}
		}


		/// <summary>
		///     Binds the <see cref="UIElement.PreviewKeyDown" /> event to a command.
		/// </summary>
		public static readonly DependencyProperty PreviewKeyDownEventProperty = DependencyProperty.RegisterAttached("PreviewKeyDownEvent", typeof(ICommand), typeof(EventBinder), new FrameworkPropertyMetadata(EventBinder.OnUIElementPreviewKeyDownEventChanged));

		private static KeyEventHandler UIElementPreviewKeyDownEventHandler { get; set; } = (s, e) =>
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

		private static void OnUIElementPreviewKeyDownEventChanged (DependencyObject obj, DependencyPropertyChangedEventArgs args)
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
				control.PreviewKeyDown -= EventBinder.UIElementPreviewKeyDownEventHandler;
			}

			if (newCommand != null)
			{
				control.PreviewKeyDown += EventBinder.UIElementPreviewKeyDownEventHandler;
			}
		}


		/// <summary>
		///     Binds the <see cref="UIElement.PreviewKeyUp" /> event to a command.
		/// </summary>
		public static readonly DependencyProperty PreviewKeyUpEventProperty = DependencyProperty.RegisterAttached("PreviewKeyUpEvent", typeof(ICommand), typeof(EventBinder), new FrameworkPropertyMetadata(EventBinder.OnUIElementPreviewKeyUpEventChanged));

		private static KeyEventHandler UIElementPreviewKeyUpEventHandler { get; set; } = (s, e) =>
		{
			ICommand command = EventBinder.GetPreviewKeyUpEvent(s as DependencyObject);
			if (command != null)
			{
				if (command.CanExecute(e))
				{
					command.Execute(e);
				}
			}
		};

		/// <summary>
		///     Gets the command bound to the <see cref="UIElement.PreviewKeyUp" /> event.
		/// </summary>
		/// <param name="obj"> The container. </param>
		/// <returns>
		///     The command bound to the <see cref="UIElement.PreviewKeyUp" /> event.
		/// </returns>
		/// <remarks>
		///     <note type="note">
		///         This method is for supporting the XAML designer and not intended to be used by your code.
		///     </note>
		/// </remarks>
		public static ICommand GetPreviewKeyUpEvent (DependencyObject obj)
		{
			return obj?.GetValue(EventBinder.PreviewKeyUpEventProperty) as ICommand;
		}

		/// <summary>
		///     Sets the command bound to the <see cref="UIElement.PreviewKeyUp" /> event.
		/// </summary>
		/// <param name="obj"> The container. </param>
		/// <param name="value"> The command to bind to the <see cref="UIElement.PreviewKeyUp" /> event. </param>
		/// <remarks>
		///     <note type="note">
		///         This method is for supporting the XAML designer and not intended to be used by your code.
		///     </note>
		/// </remarks>
		public static void SetPreviewKeyUpEvent (DependencyObject obj, ICommand value)
		{
			obj?.SetValue(EventBinder.PreviewKeyUpEventProperty, value);
		}

		private static void OnUIElementPreviewKeyUpEventChanged (DependencyObject obj, DependencyPropertyChangedEventArgs args)
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
				control.PreviewKeyUp -= EventBinder.UIElementPreviewKeyUpEventHandler;
			}

			if (newCommand != null)
			{
				control.PreviewKeyUp += EventBinder.UIElementPreviewKeyUpEventHandler;
			}
		}


		/// <summary>
		///     Binds the <see cref="UIElement.KeyDown" /> event to a command.
		/// </summary>
		public static readonly DependencyProperty KeyDownEventProperty = DependencyProperty.RegisterAttached("KeyDownEvent", typeof(ICommand), typeof(EventBinder), new FrameworkPropertyMetadata(EventBinder.OnUIElementKeyDownEventChanged));

		private static KeyEventHandler UIElementKeyDownEventHandler { get; set; } = (s, e) =>
		{
			ICommand command = EventBinder.GetKeyDownEvent(s as DependencyObject);
			if (command != null)
			{
				if (command.CanExecute(e))
				{
					command.Execute(e);
				}
			}
		};

		/// <summary>
		///     Gets the command bound to the <see cref="UIElement.KeyDown" /> event.
		/// </summary>
		/// <param name="obj"> The container. </param>
		/// <returns>
		///     The command bound to the <see cref="UIElement.KeyDown" /> event.
		/// </returns>
		/// <remarks>
		///     <note type="note">
		///         This method is for supporting the XAML designer and not intended to be used by your code.
		///     </note>
		/// </remarks>
		public static ICommand GetKeyDownEvent (DependencyObject obj)
		{
			return obj?.GetValue(EventBinder.KeyDownEventProperty) as ICommand;
		}

		/// <summary>
		///     Sets the command bound to the <see cref="UIElement.KeyDown" /> event.
		/// </summary>
		/// <param name="obj"> The container. </param>
		/// <param name="value"> The command to bind to the <see cref="UIElement.KeyDown" /> event. </param>
		/// <remarks>
		///     <note type="note">
		///         This method is for supporting the XAML designer and not intended to be used by your code.
		///     </note>
		/// </remarks>
		public static void SetKeyDownEvent (DependencyObject obj, ICommand value)
		{
			obj?.SetValue(EventBinder.KeyDownEventProperty, value);
		}

		private static void OnUIElementKeyDownEventChanged (DependencyObject obj, DependencyPropertyChangedEventArgs args)
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
				control.KeyDown -= EventBinder.UIElementKeyDownEventHandler;
			}

			if (newCommand != null)
			{
				control.KeyDown += EventBinder.UIElementKeyDownEventHandler;
			}
		}


		/// <summary>
		///     Binds the <see cref="UIElement.KeyUp" /> event to a command.
		/// </summary>
		public static readonly DependencyProperty KeyUpEventProperty = DependencyProperty.RegisterAttached("KeyUpEvent", typeof(ICommand), typeof(EventBinder), new FrameworkPropertyMetadata(EventBinder.OnUIElementKeyUpEventChanged));

		private static KeyEventHandler UIElementKeyUpEventHandler { get; set; } = (s, e) =>
		{
			ICommand command = EventBinder.GetKeyUpEvent(s as DependencyObject);
			if (command != null)
			{
				if (command.CanExecute(e))
				{
					command.Execute(e);
				}
			}
		};

		/// <summary>
		///     Gets the command bound to the <see cref="UIElement.KeyUp" /> event.
		/// </summary>
		/// <param name="obj"> The container. </param>
		/// <returns>
		///     The command bound to the <see cref="UIElement.KeyUp" /> event.
		/// </returns>
		/// <remarks>
		///     <note type="note">
		///         This method is for supporting the XAML designer and not intended to be used by your code.
		///     </note>
		/// </remarks>
		public static ICommand GetKeyUpEvent (DependencyObject obj)
		{
			return obj?.GetValue(EventBinder.KeyUpEventProperty) as ICommand;
		}

		/// <summary>
		///     Sets the command bound to the <see cref="UIElement.KeyUp" /> event.
		/// </summary>
		/// <param name="obj"> The container. </param>
		/// <param name="value"> The command to bind to the <see cref="UIElement.KeyUp" /> event. </param>
		/// <remarks>
		///     <note type="note">
		///         This method is for supporting the XAML designer and not intended to be used by your code.
		///     </note>
		/// </remarks>
		public static void SetKeyUpEvent (DependencyObject obj, ICommand value)
		{
			obj?.SetValue(EventBinder.KeyUpEventProperty, value);
		}

		private static void OnUIElementKeyUpEventChanged (DependencyObject obj, DependencyPropertyChangedEventArgs args)
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
				control.KeyUp -= EventBinder.UIElementKeyUpEventHandler;
			}

			if (newCommand != null)
			{
				control.KeyUp += EventBinder.UIElementKeyUpEventHandler;
			}
		}

	}
}
