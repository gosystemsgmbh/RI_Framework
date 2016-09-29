using System.Windows;




namespace RI.Framework.Services.Regions
{
	/// <summary>
	///     Implements a base class for WPF region adapters which provides some commonly used base functionality.
	/// </summary>
	public abstract class WpfRegionAdapterBase : RegionAdapterBase
	{
		#region Overrides

		/// <inheritdoc />
		public override void Activate (object container, object element)
		{
			base.Activate(container, element);

			if (element is FrameworkElement)
			{
				FrameworkElement frameworkElement = (FrameworkElement)element;
				if (frameworkElement.DataContext is IRegionElement)
				{
					IRegionElement regionElement = (IRegionElement)frameworkElement.DataContext;
					regionElement.Activated();
				}
			}
		}

		/// <inheritdoc />
		public override void Deactivate (object container, object element)
		{
			base.Deactivate(container, element);

			if (element is FrameworkElement)
			{
				FrameworkElement frameworkElement = (FrameworkElement)element;
				if (frameworkElement.DataContext is IRegionElement)
				{
					IRegionElement regionElement = (IRegionElement)frameworkElement.DataContext;
					regionElement.Deactivated();
				}
			}
		}

		#endregion
	}
}
