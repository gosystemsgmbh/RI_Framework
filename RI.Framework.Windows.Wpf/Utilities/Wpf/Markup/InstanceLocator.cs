using System;
using System.Windows;
using System.Windows.Markup;

using RI.Framework.Composition;
using RI.Framework.Mvvm.View;
using RI.Framework.Mvvm.ViewModel;
using RI.Framework.Services;
using RI.Framework.Services.Logging;




namespace RI.Framework.Utilities.Wpf.Markup
{
	/// <summary>
	///     Implements a WPF XAML markup extension to obtain instances.
	/// </summary>
	/// <remarks>
	///     <para>
	///         The <see cref="InstanceLocator" /> is used in XAML to get instances from <see cref="ServiceLocator" /> and assign them to properties in XAML.
	///         For example, this can be used to retrieve and attach a view model to a <see cref="FrameworkElement.DataContext" /> in MVVM scenarios.
	///     </para>
	///     <para>
	///         The instance to obtain can be either specified by its name, using the <see cref="Name" /> property, or its type, using the <see cref="Type" /> property.
	///         See <see cref="ServiceLocator" /> for more details.
	///     </para>
	/// </remarks>
	[MarkupExtensionReturnType(typeof(object))]
	public sealed class InstanceLocator : MarkupExtension
	{
		#region Static Methods

		private static object GetValue (string name)
		{
			if (name == null)
			{
				return null;
			}

			if (name.IsEmptyOrWhitespace())
			{
				return null;
			}

			object value = ServiceLocator.GetInstance(name);
			InstanceLocator.ProcessValue(value);
			return value;
		}

		private static object GetValue (Type type)
		{
			if (type == null)
			{
				return null;
			}

			object value = ServiceLocator.GetInstance(type);
			InstanceLocator.ProcessValue(value);
			return value;
		}

		private static void ProcessValue (object value)
		{
			if (value == null)
			{
				return;
			}

			CompositionContainer container = ServiceLocator.GetInstance<CompositionContainer>();

			if (container != null)
			{
				container.ResolveImports(value, CompositionFlags.Normal);
			}
			if (value is IViewModel)
			{
				IViewModel viewModel = (IViewModel)value;
				if (!viewModel.IsInitialized)
				{
					viewModel.Initialize();
				}
			}
			if (value is IView)
			{
				IView view = (IView)value;
				if (!view.IsInitialized)
				{
					view.Initialize();
				}
			}

			if (value is FrameworkElement)
			{
				FrameworkElement frameworkElement = (FrameworkElement)value;
				if (frameworkElement.DataContext != null)
				{
					if (container != null)
					{
						container.ResolveImports(frameworkElement.DataContext, CompositionFlags.Normal);
					}
				}
				if (frameworkElement.DataContext is IViewModel)
				{
					IViewModel viewModel = (IViewModel)frameworkElement.DataContext;
					if (!viewModel.IsInitialized)
					{
						viewModel.Initialize();
					}
				}
				if (frameworkElement.DataContext is IView)
				{
					IView view = (IView)frameworkElement.DataContext;
					if (!view.IsInitialized)
					{
						view.Initialize();
					}
				}
			}
		}

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="InstanceLocator" />.
		/// </summary>
		public InstanceLocator ()
		{
			this.Name = null;
			this.Type = null;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     The name of the instance to obtain.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///     The type of the instance to obtain.
		/// </summary>
		public Type Type { get; set; }

		#endregion




		#region Overrides

		/// <inheritdoc />
		public override object ProvideValue (IServiceProvider serviceProvider)
		{
			object value = InstanceLocator.GetValue(this.Name) ?? InstanceLocator.GetValue(this.Type);
			if (value == null)
			{
				LogLocator.LogWarning(this.GetType().Name, "No value available while trying obtain instance: Name={0}, Type={1}", this.Name ?? "[null]", this.Type?.Name ?? "[null]");
			}

			IProvideValueTarget targetProvider = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
			if (targetProvider != null)
			{
				object target = targetProvider.TargetObject;
				InstanceLocator.ProcessValue(target);
			}

			return value;
		}

		#endregion
	}
}
