using System.Windows;




namespace RI.Framework.Services.Regions
{
	/// <summary>
	///     Provides WPF XAML extensions to work with regions.
	/// </summary>
	public static class RegionExtension
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
		public static readonly DependencyProperty RegionNameProperty = DependencyProperty.RegisterAttached("RegionName", typeof(string), typeof(RegionExtension), new UIPropertyMetadata(null, RegionExtension.OnRegionNameChange));

		#endregion




		#region Static Methods

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
