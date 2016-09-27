using System.Windows;




namespace RI.Framework.Services.Regions
{
	/// <summary>
	///     Provides WPF XAML extensions to work with regions.
	/// </summary>
	public static class WpfRegionExtension
	{
		#region Constants

		/// <summary>
		///     Associates the object with a region by specifying its region name.
		/// </summary>
		/// <remarks>
		///     <para>
		///         This property uses <see cref="ServiceLocator" /> to obtain an instance to <see cref="IRegionService" /> and then calls <see cref="IRegionService.RemoveRegion" /> and <see cref="IRegionService.AddRegion" />.
		///     </para>
		/// </remarks>
		public static readonly DependencyProperty RegionNameProperty = DependencyProperty.RegisterAttached("RegionName", typeof(string), typeof(WpfRegionExtension), new UIPropertyMetadata(null, WpfRegionExtension.OnRegionNameChange));

		#endregion




		#region Static Methods

		/// <summary>
		///     Gets the region name of the specified container.
		/// </summary>
		/// <param name="obj"> The container. </param>
		/// <returns>
		///     The region name associated with the container.
		/// </returns>
		public static string GetRegionName (DependencyObject obj)
		{
			return ((string)obj.GetValue(WpfRegionExtension.RegionNameProperty));
		}

		/// <summary>
		///     Sets the region name of the specified container.
		/// </summary>
		/// <param name="obj"> The container. </param>
		/// <param name="value"> The region name. </param>
		public static void SetRegionName (DependencyObject obj, string value)
		{
			obj.SetValue(WpfRegionExtension.RegionNameProperty, value);
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
