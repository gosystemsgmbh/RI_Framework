﻿using System;
using System.Windows;

using RI.Framework.Services;
using RI.Framework.Services.Logging;
using RI.Framework.Services.Regions;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Mvvm
{
	/// <summary>
	///     Provides WPF XAML extensions and utilities to work with regions.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="RegionNavigator" /> is a convenience utility to work with regions (<see cref="IRegionService" /> in particular) in MVVM scenarios.
	///         It defines an attached property (<see cref="RegionNameProperty" />) which can be used in XAML to associate a container with a region (using the region services <see cref="IRegionService.AddRegion" /> method).
	///         It also defines navigation methods to simplify navigation of elements inside containers.
	///     </para>
	///     <para>
	///         <see cref="ServiceLocator" /> is used to obtain an instance of <see cref="IRegionService" />.
	///     </para>
	///     <para>
	///         To obtain the instances for the elements when navigating, <see cref="InstanceLocator" /> is used.
	///     </para>
	/// </remarks>
	public static class RegionNavigator
	{
		#region Constants

		/// <summary>
		///     Associates a container with a region by specifying its region name.
		/// </summary>
		public static readonly DependencyProperty RegionNameProperty = DependencyProperty.RegisterAttached("RegionName", typeof(string), typeof(RegionNavigator), new UIPropertyMetadata(null, RegionNavigator.OnRegionNameChange));

		#endregion




		#region Static Methods

		/// <summary>
		///     Activates an element of a specified name in the specified region.
		/// </summary>
		/// <param name="region"> The region. </param>
		/// <param name="element"> The name of the element. </param>
		/// <remarks>
		///     <para>
		///         See <see cref="Activate(string,object)" /> for more details.
		///     </para>
		///     <para>
		///         The elements instance is obtaines using <see cref="InstanceLocator" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> or <paramref name="element" /> is an empty string. </exception>
		/// <exception cref="InvalidOperationException"> The region specified by <paramref name="region" /> or the element specified by <paramref name="element" /> does not exist or no region service is available. </exception>
		public static void Activate (string region, string element)
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

			object value = InstanceLocator.GetValue(element);
			if (value == null)
			{
				throw new InvalidOperationException();
			}

			RegionNavigator.Activate(region, value);
		}

		/// <summary>
		///     Activates an element of a specified type in the specified region.
		/// </summary>
		/// <param name="region"> The region. </param>
		/// <param name="element"> The type of the element. </param>
		/// <remarks>
		///     <para>
		///         See <see cref="Activate(string,object)" /> for more details.
		///     </para>
		///     <para>
		///         The elements instance is obtaines using <see cref="InstanceLocator" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> is an empty string. </exception>
		/// <exception cref="InvalidOperationException"> The region specified by <paramref name="region" /> or the element specified by <paramref name="element" /> does not exist or no region service is available. </exception>
		public static void Activate (string region, Type element)
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

			object value = InstanceLocator.GetValue(element);
			if (value == null)
			{
				throw new InvalidOperationException();
			}

			RegionNavigator.Activate(region, value);
		}

		/// <summary>
		///     Activates an element in the specified region.
		/// </summary>
		/// <param name="region"> The region. </param>
		/// <param name="element"> The element. </param>
		/// <remarks>
		///     <para>
		///         <see cref="IRegionService" />.<see cref="IRegionService.ActivateElement" /> is used internally, retrieved using <see cref="ServiceLocator" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> is an empty string. </exception>
		/// <exception cref="InvalidOperationException"> The region specified by <paramref name="region" /> does not exist or no region service is available. </exception>
		public static void Activate (string region, object element)
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
				throw new InvalidOperationException();
			}

			object container = regionService.GetRegionContainer(region);

			InstanceLocator.ProcessValue(container);
			InstanceLocator.ProcessValue(element);

			regionService.ActivateElement(region, element);
		}

		/// <summary>
		///     Adds an element of a specified name to the specified region.
		/// </summary>
		/// <param name="region"> The region. </param>
		/// <param name="element"> The name of the element. </param>
		/// <remarks>
		///     <para>
		///         See <see cref="Add(string,object)" /> for more details.
		///     </para>
		///     <para>
		///         The elements instance is obtaines using <see cref="InstanceLocator" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> or <paramref name="element" /> is an empty string. </exception>
		/// <exception cref="InvalidOperationException"> The region specified by <paramref name="region" /> or the element specified by <paramref name="element" /> does not exist or no region service is available. </exception>
		public static void Add (string region, string element)
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

			object value = InstanceLocator.GetValue(element);
			if (value == null)
			{
				throw new InvalidOperationException();
			}

			RegionNavigator.Add(region, value);
		}

		/// <summary>
		///     Adds an element of a specified type to the specified region.
		/// </summary>
		/// <param name="region"> The region. </param>
		/// <param name="element"> The type of the element. </param>
		/// <remarks>
		///     <para>
		///         See <see cref="Add(string,object)" /> for more details.
		///     </para>
		///     <para>
		///         The elements instance is obtaines using <see cref="InstanceLocator" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> is an empty string. </exception>
		/// <exception cref="InvalidOperationException"> The region specified by <paramref name="region" /> or the element specified by <paramref name="element" /> does not exist or no region service is available. </exception>
		public static void Add (string region, Type element)
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

			object value = InstanceLocator.GetValue(element);
			if (value == null)
			{
				throw new InvalidOperationException();
			}

			RegionNavigator.Add(region, value);
		}

		/// <summary>
		///     Adds an element to the specified region.
		/// </summary>
		/// <param name="region"> The region. </param>
		/// <param name="element"> The element. </param>
		/// <remarks>
		///     <para>
		///         <see cref="IRegionService" />.<see cref="IRegionService.AddElement" /> is used internally, retrieved using <see cref="ServiceLocator" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> is an empty string. </exception>
		/// <exception cref="InvalidOperationException"> The region specified by <paramref name="region" /> does not exist or no region service is available. </exception>
		public static void Add (string region, object element)
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
				throw new InvalidOperationException();
			}

			object container = regionService.GetRegionContainer(region);

			InstanceLocator.ProcessValue(container);
			InstanceLocator.ProcessValue(element);

			regionService.AddElement(region, element);
		}

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
			return obj?.GetValue(RegionNavigator.RegionNameProperty) as string;
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
			obj?.SetValue(RegionNavigator.RegionNameProperty, value);
		}

		private static void OnRegionNameChange (DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			string oldRegion = e.OldValue as string;
			string newRegion = e.NewValue as string;

			IRegionService regionService = ServiceLocator.GetInstance<IRegionService>();
			if (regionService == null)
			{
				LogLocator.LogWarning(typeof(RegionNavigator).Name, "No region service available while trying to assign region: {0}/{1} -> {2}", oldRegion ?? "[null]", newRegion ?? "[null]", obj.GetType().Name);
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

			InstanceLocator.ProcessValue(obj);
		}

		#endregion
	}
}