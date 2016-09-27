using System;
using System.Windows;

using RI.Framework.Mvvm;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Services.Regions
{
	/// <summary>
	///     Provides WPF XAML extensions and utilities to work with regions.
	/// </summary>
	public static class RegionNavigator
	{
		#region Constants

		/// <summary>
		///     Associates a container with a region by specifying its region name.
		/// </summary>
		/// <remarks>
		///     <para>
		///         This property uses <see cref="ServiceLocator" /> to obtain an instance to <see cref="IRegionService" /> and then calls <see cref="IRegionService.RemoveRegion" /> and <see cref="IRegionService.AddRegion" />.
		///     </para>
		/// </remarks>
		public static readonly DependencyProperty RegionNameProperty = DependencyProperty.RegisterAttached("RegionName", typeof(string), typeof(RegionNavigator), new UIPropertyMetadata(null, RegionNavigator.OnRegionNameChange));

		#endregion




		#region Static Methods

		/// <summary>
		///     Gets the region name of the specified container.
		/// </summary>
		/// <param name="obj"> The container. </param>
		/// <returns>
		///     The region name associated with the container or null if the container has no region name associated.
		/// </returns>
		/// <remarks>
		///     See <see cref="RegionNameProperty" /> for more details.
		/// </remarks>
		public static string GetRegionName (DependencyObject obj)
		{
			return obj?.GetValue(RegionNavigator.RegionNameProperty) as string;
		}

		public static void Navigate (string region, string element)
		{
			if (region == null)
			{
				throw new ArgumentNullException(nameof(region));
			}

			if (region.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(region));
			}

			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}

			if (element.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(element));
			}

			object value = ObjectLocator.GetValue(element);
			RegionNavigator.Navigate(region, value);
		}

		public static void Navigate (string region, Type element)
		{
			if (region == null)
			{
				throw new ArgumentNullException(nameof(region));
			}

			if (region.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(region));
			}

			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}

			object value = ObjectLocator.GetValue(element);
			RegionNavigator.Navigate(region, value);
		}

		public static void Navigate (string region, object element)
		{
			if (region == null)
			{
				throw new ArgumentNullException(nameof(region));
			}

			if (region.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(region));
			}

			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}

			IRegionService regionService = ServiceLocator.GetInstance<IRegionService>();
			if (regionService == null)
			{
				return;
			}

			object container = regionService.GetRegionContainer(region);

			ObjectLocator.ProcessValue(container);
			ObjectLocator.ProcessValue(element);

			regionService.ActivateElement(region, element);
		}

		/// <summary>
		///     Sets the region name of the specified container.
		/// </summary>
		/// <param name="obj"> The container. </param>
		/// <param name="value"> The region name (can be null to unassociate the container from the region). </param>
		/// <remarks>
		///     See <see cref="RegionNameProperty" /> for more details.
		/// </remarks>
		public static void SetRegionName (DependencyObject obj, string value)
		{
			obj?.SetValue(RegionNavigator.RegionNameProperty, value);
		}


		private static void OnRegionNameChange (DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			string oldRegion = e.OldValue as string;
			string newRegion = e.NewValue as string;

			IRegionService regionService = ServiceLocator.GetInstance<IRegionService>();
			if (regionService == null)
			{
				return;
			}

			if (oldRegion != null)
			{
				regionService.RemoveRegion(oldRegion);
			}

			if (newRegion != null)
			{
				regionService.AddRegion(newRegion, obj);
			}
		}

		#endregion
	}
}
