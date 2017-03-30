using System.Windows;

using RI.Framework.Services;
using RI.Framework.Services.Logging;
using RI.Framework.Services.Regions;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Utilities.Wpf.Markup
{
	/// <summary>
	///     Provides attached properties to associate a WPF control with a region of a <see cref="IRegionService"/>.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="RegionBinder" /> is a convenience utility to work with regions (<see cref="IRegionService" />) in MVVM scenarios.
	///         It defines an attached property (<see cref="RegionNameProperty" />) which can be used to associate a container with a region (using the region services <see cref="IRegionService.AddRegion" /> method).
	///         It also defines region operation methods to simplify region handling.
	///     </para>
	///     <para>
	///         <see cref="ServiceLocator" /> or <see cref="Singleton{IRegionService}"/> is used to obtain an instance of <see cref="IRegionService" />.
	///     </para>
	/// </remarks>
	public static class RegionBinder
	{
		#region Constants

		/// <summary>
		///     Associates a container with a region by specifying its region name.
		/// </summary>
		public static readonly DependencyProperty RegionNameProperty = DependencyProperty.RegisterAttached("RegionName", typeof(string), typeof(RegionBinder), new UIPropertyMetadata(null, RegionBinder.OnRegionNameChange));

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
		///     <note type="note">
		///         This method is for supporting the XAML designer and not intended to be used by your code.
		///     </note>
		/// </remarks>
		public static string GetRegionName (DependencyObject obj)
		{
			return obj?.GetValue(RegionBinder.RegionNameProperty) as string;
		}

		/// <summary>
		///     Sets the region name of the specified container.
		/// </summary>
		/// <param name="obj"> The container. </param>
		/// <param name="value"> The region name to associate with the container. Can be null to unassociate the container from a region. </param>
		/// <remarks>
		///     <note type="note">
		///         This method is for supporting the XAML designer and not intended to be used by your code.
		///     </note>
		/// </remarks>
		public static void SetRegionName (DependencyObject obj, string value)
		{
			obj?.SetValue(RegionBinder.RegionNameProperty, value);
		}

		private static void OnRegionNameChange (DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			string oldRegion = e.OldValue as string;
			string newRegion = e.NewValue as string;

			IRegionService regionService = ServiceLocator.GetInstance<IRegionService>() ?? Singleton<IRegionService>.Instance;
			if (regionService == null)
			{
				LogLocator.LogWarning(typeof(RegionBinder).Name, "No region service available while trying to assign region: Object={0}, OldRegion={1}, NewRegion={2}", obj?.GetType().Name ?? "[null]", oldRegion ?? "[null]", newRegion ?? "[null]");
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
