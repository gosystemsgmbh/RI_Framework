using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace RI.Framework.Utilities.Wpf.Markup
{
	public sealed class EventBinding : Freezable
	{
		public EventBinding()
		{
			this.AttachedTo = null;
			this.EventInfo = null;
			this.EventHandlerDelegate = null;

			this.EventHandlerMethod = this.GetType().GetMethod(nameof(this.OnEvent), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		}

		private DependencyObject AttachedTo { get; set; }
		private EventInfo EventInfo { get; set; }
		private Delegate EventHandlerDelegate { get; set; }
		private MethodInfo EventHandlerMethod { get; set; }

		public static DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(EventBinding), new UIPropertyMetadata(null, EventBinding.OnCommandChanged));
		public static DependencyProperty EventNameProperty = DependencyProperty.Register("EventName", typeof(string), typeof(EventBinding), new UIPropertyMetadata(null, EventBinding.OnEventNameChanged));

		private static void OnCommandChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			(obj as EventBinding)?.ReAttach();
		}
		private static void OnEventNameChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			(obj as EventBinding)?.ReAttach();
		}

		public ICommand Command
		{
			get
			{
				return (ICommand)this.GetValue(EventBinding.CommandProperty);
			}
			set
			{
				this.SetValue(EventBinding.CommandProperty, value);
			}
		}
		public string EventName
		{
			get
			{
				return (string)this.GetValue(EventBinding.EventNameProperty);
			}
			set
			{
				this.SetValue(EventBinding.EventNameProperty, value);
			}
		}

		internal void ReAttach()
		{
			this.Attach(this.AttachedTo);
		}

		internal void Attach(DependencyObject attachedTo)
		{
			this.Detach();

			//TODO: Check event name
			if (this.EventName == null)
			{
				return;
			}

			this.AttachedTo = attachedTo;
			if (this.AttachedTo == null)
			{
				return;
			}

			this.EventInfo = this.AttachedTo.GetType().GetEvent(this.EventName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (this.EventInfo == null)
			{
				return;
			}

			this.EventHandlerDelegate = Delegate.CreateDelegate(this.EventInfo.EventHandlerType, this, this.EventHandlerMethod);

			this.EventInfo.AddEventHandler(this.AttachedTo, this.EventHandlerDelegate);
		}

		internal void Detach()
		{
			this.EventInfo?.RemoveEventHandler(this.AttachedTo, this.EventHandlerDelegate);

			this.EventHandlerDelegate = null;
			this.EventInfo = null;
			this.AttachedTo = null;
		}

		private void OnEvent(object sender, object eventArgs)
		{
			if (this.Command == null)
			{
				return;
			}

			if (!this.Command.CanExecute(eventArgs))
			{
				return;
			}

			this.Command.Execute(eventArgs);
		}

		protected override Freezable CreateInstanceCore()
		{
			return new EventBinding();
		}
	}

	public sealed class EventBindings : FreezableCollection<EventBinding>
	{
		private DependencyObject AttachedTo { get; }

		public EventBindings(DependencyObject attachedTo)
		{
			if (attachedTo == null)
			{
				throw new ArgumentNullException(nameof(attachedTo));
			}

			this.AttachedTo = attachedTo;

			((INotifyCollectionChanged)this).CollectionChanged += (sender, args) =>
			{
				if (args.OldItems != null)
				{
					foreach (object oldItem in args.OldItems)
					{
						(oldItem as EventBinding)?.Detach();
					}
				}

				if (args.NewItems != null)
				{
					foreach (object newItem in args.NewItems)
					{
						(newItem as EventBinding)?.Attach(this.AttachedTo);
					}
				}
			};
		}
	}

	public static class EventToCommandBinder
	{
		public static readonly DependencyProperty EventBindingsProperty = DependencyProperty.RegisterAttached("EventBindingsInternal", typeof(EventBindings), typeof(EventToCommandBinder), new UIPropertyMetadata(null, EventToCommandBinder.OnEventBindingsChanged));

		public static EventBindings GetEventBindings(DependencyObject item)
		{
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			EventBindings collection = item.GetValue(EventToCommandBinder.EventBindingsProperty) as EventBindings;

			if (collection == null)
			{
				collection = new EventBindings(item);
				item.SetValue(EventToCommandBinder.EventBindingsProperty, collection);
			}

			return collection;
		}

		private static void OnEventBindingsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
		}
	}
}
