using System;
using System.Windows;

using RI.Framework.Services;
using RI.Framework.Services.Logging;
using RI.Framework.Services.Regions;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Mvvm
{
	/// <summary>
	///     Provides utilities to work with regions.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="RegionManager" /> is a convenience utility to work with regions (<see cref="IRegionService" />) in MVVM scenarios.
	///         It defines an attached property (<see cref="RegionNameProperty" />) which can be used to associate a container with a region (using the region services <see cref="IRegionService.AddRegion" /> method).
	///         It also defines region operation methods to simplify region handling.
	///     </para>
	///     <para>
	///         <see cref="ServiceLocator" /> is used to obtain an instance of <see cref="IRegionService" /> and, when using region operation methods, the instances for elements when the elements are specified by name or type.
	///     </para>
	/// </remarks>
	public static class RegionManager
	{
		#region Constants

		/// <summary>
		///     Associates a container with a region by specifying its region name.
		/// </summary>
		public static readonly DependencyProperty RegionNameProperty = DependencyProperty.RegisterAttached("RegionName", typeof(string), typeof(RegionManager), new UIPropertyMetadata(null, RegionManager.OnRegionNameChange));

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
		/// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
		/// <exception cref="InvalidOperationException"> <paramref name="element" /> cannot be resolved or no region service is available. </exception>
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

			object value = RegionManager.GetValue(element);
			if (value == null)
			{
				throw new InvalidOperationException();
			}

			RegionManager.Activate(region, value);
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
		/// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
		/// <exception cref="InvalidOperationException"> <paramref name="element" /> cannot be resolved or no region service is available. </exception>
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

			object value = RegionManager.GetValue(element);
			if (value == null)
			{
				throw new InvalidOperationException();
			}

			RegionManager.Activate(region, value);
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
		/// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
		/// <exception cref="InvalidOperationException"> No region service is available. </exception>
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
		/// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
		/// <exception cref="InvalidOperationException"> <paramref name="element" /> cannot be resolved or no region service is available. </exception>
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

			object value = RegionManager.GetValue(element);
			if (value == null)
			{
				throw new InvalidOperationException();
			}

			RegionManager.Add(region, value);
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
		/// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
		/// <exception cref="InvalidOperationException"> <paramref name="element" /> cannot be resolved or no region service is available. </exception>
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

			object value = RegionManager.GetValue(element);
			if (value == null)
			{
				throw new InvalidOperationException();
			}

			RegionManager.Add(region, value);
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
		/// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
		/// <exception cref="InvalidOperationException"> No region service is available. </exception>
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

			regionService.AddElement(region, element);
		}

		/// <summary>
		///     Checks whether it is possible to navigate to an element of a specified name in the specified region.
		/// </summary>
		/// <param name="region"> The region. </param>
		/// <param name="element"> The name of the element. </param>
		/// <returns>
		///     true if the navigation is possible, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         See <see cref="Navigate(string,object)" /> for more details.
		///     </para>
		///     <para>
		///         The elements instance is obtaines using <see cref="InstanceLocator" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> or <paramref name="element" /> is an empty string. </exception>
		/// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
		/// <exception cref="InvalidOperationException"> <paramref name="element" /> cannot be resolved or no region service is available. </exception>
		public static bool CanNavigate (string region, string element)
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

			object value = RegionManager.GetValue(element);
			if (value == null)
			{
				throw new InvalidOperationException();
			}

			return RegionManager.CanNavigate(region, value);
		}

		/// <summary>
		///     Checks whether it is possible to navigate to an element of a specified type in the specified region.
		/// </summary>
		/// <param name="region"> The region. </param>
		/// <param name="element"> The type of the element. </param>
		/// <returns>
		///     true if the navigation is possible, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         See <see cref="Navigate(string,object)" /> for more details.
		///     </para>
		///     <para>
		///         The elements instance is obtaines using <see cref="InstanceLocator" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> is an empty string. </exception>
		/// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
		/// <exception cref="InvalidOperationException"> <paramref name="element" /> cannot be resolved or no region service is available. </exception>
		public static bool CanNavigate (string region, Type element)
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

			object value = RegionManager.GetValue(element);
			if (value == null)
			{
				throw new InvalidOperationException();
			}

			return RegionManager.CanNavigate(region, value);
		}

		/// <summary>
		///     Checks whether it is possible to navigate to an element in the specified region.
		/// </summary>
		/// <param name="region"> The region. </param>
		/// <param name="element"> The element. </param>
		/// <returns>
		///     true if the navigation is possible, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         <see cref="IRegionService" />.<see cref="IRegionService.Navigate" /> is used internally, retrieved using <see cref="ServiceLocator" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> is an empty string. </exception>
		/// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
		/// <exception cref="InvalidOperationException"> No region service is available. </exception>
		public static bool CanNavigate (string region, object element)
		{
			if (region == null)
			{
				throw new ArgumentNullException(nameof(region));
			}

			if (region.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(region));
			}

			IRegionService regionService = ServiceLocator.GetInstance<IRegionService>();
			if (regionService == null)
			{
				throw new InvalidOperationException();
			}

			return regionService.CanNavigate(region, element);
		}

		/// <summary>
		///     Removes all elements from the specified region.
		/// </summary>
		/// <param name="region"> The region. </param>
		/// <remarks>
		///     <para>
		///         <see cref="IRegionService" />.<see cref="IRegionService.ClearElements" /> is used internally, retrieved using <see cref="ServiceLocator" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> is an empty string. </exception>
		/// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
		/// <exception cref="InvalidOperationException"> No region service is available. </exception>
		public static void Clear (string region)
		{
			if (region == null)
			{
				throw new ArgumentNullException(nameof(region));
			}

			if (region.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(region));
			}

			IRegionService regionService = ServiceLocator.GetInstance<IRegionService>();
			if (regionService == null)
			{
				throw new InvalidOperationException();
			}

			regionService.ClearElements(region);
		}

		/// <summary>
		///     Deactivates an element of a specified name in the specified region.
		/// </summary>
		/// <param name="region"> The region. </param>
		/// <param name="element"> The name of the element. </param>
		/// <remarks>
		///     <para>
		///         See <see cref="Deactivate(string,object)" /> for more details.
		///     </para>
		///     <para>
		///         The elements instance is obtaines using <see cref="InstanceLocator" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> or <paramref name="element" /> is an empty string. </exception>
		/// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
		/// <exception cref="InvalidOperationException"> <paramref name="element" /> cannot be resolved or no region service is available. </exception>
		public static void Deactivate (string region, string element)
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

			object value = RegionManager.GetValue(element);
			if (value == null)
			{
				throw new InvalidOperationException();
			}

			RegionManager.Deactivate(region, value);
		}

		/// <summary>
		///     Deactivates an element of a specified type in the specified region.
		/// </summary>
		/// <param name="region"> The region. </param>
		/// <param name="element"> The type of the element. </param>
		/// <remarks>
		///     <para>
		///         See <see cref="Deactivate(string,object)" /> for more details.
		///     </para>
		///     <para>
		///         The elements instance is obtaines using <see cref="InstanceLocator" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> is an empty string. </exception>
		/// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
		/// <exception cref="InvalidOperationException"> <paramref name="element" /> cannot be resolved or no region service is available. </exception>
		public static void Deactivate (string region, Type element)
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

			object value = RegionManager.GetValue(element);
			if (value == null)
			{
				throw new InvalidOperationException();
			}

			RegionManager.Deactivate(region, value);
		}

		/// <summary>
		///     Deactivates an element in the specified region.
		/// </summary>
		/// <param name="region"> The region. </param>
		/// <param name="element"> The element. </param>
		/// <remarks>
		///     <para>
		///         <see cref="IRegionService" />.<see cref="IRegionService.DeactivateElement" /> is used internally, retrieved using <see cref="ServiceLocator" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> is an empty string. </exception>
		/// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
		/// <exception cref="InvalidOperationException"> No region service is available. </exception>
		public static void Deactivate (string region, object element)
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

			regionService.DeactivateElement(region, element);
		}

		/// <summary>
		///     Deactivates all elements in the specified region.
		/// </summary>
		/// <param name="region"> The region. </param>
		/// <remarks>
		///     <para>
		///         <see cref="IRegionService" />.<see cref="IRegionService.DeactivateAllElements" /> is used internally, retrieved using <see cref="ServiceLocator" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> is an empty string. </exception>
		/// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
		/// <exception cref="InvalidOperationException"> No region service is available. </exception>
		public static void DeactivateAll (string region)
		{
			if (region == null)
			{
				throw new ArgumentNullException(nameof(region));
			}

			if (region.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(region));
			}

			IRegionService regionService = ServiceLocator.GetInstance<IRegionService>();
			if (regionService == null)
			{
				throw new InvalidOperationException();
			}

			regionService.DeactivateAllElements(region);
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
			return obj?.GetValue(RegionManager.RegionNameProperty) as string;
		}

		/// <summary>
		///     Navigates to an element of a specified name in the specified region.
		/// </summary>
		/// <param name="region"> The region. </param>
		/// <param name="element"> The name of the element. </param>
		/// <returns>
		///     true if the navigation was successful, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         See <see cref="Navigate(string,object)" /> for more details.
		///     </para>
		///     <para>
		///         The elements instance is obtaines using <see cref="InstanceLocator" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> or <paramref name="element" /> is an empty string. </exception>
		/// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
		/// <exception cref="InvalidOperationException"> <paramref name="element" /> cannot be resolved or no region service is available. </exception>
		public static bool Navigate (string region, string element)
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

			object value = RegionManager.GetValue(element);
			if (value == null)
			{
				throw new InvalidOperationException();
			}

			return RegionManager.Navigate(region, value);
		}

		/// <summary>
		///     Navigates to an element of a specified type in the specified region.
		/// </summary>
		/// <param name="region"> The region. </param>
		/// <param name="element"> The type of the element. </param>
		/// <returns>
		///     true if the navigation was successful, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         See <see cref="Navigate(string,object)" /> for more details.
		///     </para>
		///     <para>
		///         The elements instance is obtaines using <see cref="InstanceLocator" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> is an empty string. </exception>
		/// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
		/// <exception cref="InvalidOperationException"> <paramref name="element" /> cannot be resolved or no region service is available. </exception>
		public static bool Navigate (string region, Type element)
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

			object value = RegionManager.GetValue(element);
			if (value == null)
			{
				throw new InvalidOperationException();
			}

			return RegionManager.Navigate(region, value);
		}

		/// <summary>
		///     Navigates to an element in the specified region.
		/// </summary>
		/// <param name="region"> The region. </param>
		/// <param name="element"> The element. </param>
		/// <returns>
		///     true if the navigation was successful, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         <see cref="IRegionService" />.<see cref="IRegionService.Navigate" /> is used internally, retrieved using <see cref="ServiceLocator" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> is an empty string. </exception>
		/// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
		/// <exception cref="InvalidOperationException"> No region service is available. </exception>
		public static bool Navigate (string region, object element)
		{
			if (region == null)
			{
				throw new ArgumentNullException(nameof(region));
			}

			if (region.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(region));
			}

			IRegionService regionService = ServiceLocator.GetInstance<IRegionService>();
			if (regionService == null)
			{
				throw new InvalidOperationException();
			}

			return regionService.Navigate(region, element);
		}

		/// <summary>
		///     Removes an element of a specified name from the specified region.
		/// </summary>
		/// <param name="region"> The region. </param>
		/// <param name="element"> The name of the element. </param>
		/// <remarks>
		///     <para>
		///         See <see cref="Remove(string,object)" /> for more details.
		///     </para>
		///     <para>
		///         The elements instance is obtaines using <see cref="InstanceLocator" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> or <paramref name="element" /> is an empty string. </exception>
		/// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
		/// <exception cref="InvalidOperationException"> <paramref name="element" /> cannot be resolved or no region service is available. </exception>
		public static void Remove (string region, string element)
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

			object value = RegionManager.GetValue(element);
			if (value == null)
			{
				throw new InvalidOperationException();
			}

			RegionManager.Remove(region, value);
		}

		/// <summary>
		///     Removes an element of a specified type from the specified region.
		/// </summary>
		/// <param name="region"> The region. </param>
		/// <param name="element"> The type of the element. </param>
		/// <remarks>
		///     <para>
		///         See <see cref="Remove(string,object)" /> for more details.
		///     </para>
		///     <para>
		///         The elements instance is obtaines using <see cref="InstanceLocator" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> is an empty string. </exception>
		/// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
		/// <exception cref="InvalidOperationException"> <paramref name="element" /> cannot be resolved or no region service is available. </exception>
		public static void Remove (string region, Type element)
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

			object value = RegionManager.GetValue(element);
			if (value == null)
			{
				throw new InvalidOperationException();
			}

			RegionManager.Remove(region, value);
		}

		/// <summary>
		///     Removes an element from the specified region.
		/// </summary>
		/// <param name="region"> The region. </param>
		/// <param name="element"> The element. </param>
		/// <remarks>
		///     <para>
		///         <see cref="IRegionService" />.<see cref="IRegionService.RemoveElement" /> is used internally, retrieved using <see cref="ServiceLocator" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> is an empty string. </exception>
		/// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
		/// <exception cref="InvalidOperationException"> No region service is available. </exception>
		public static void Remove (string region, object element)
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

			regionService.RemoveElement(region, element);
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
			obj?.SetValue(RegionManager.RegionNameProperty, value);
		}


		private static object GetValue (string name)
		{
			if (name == null)
			{
				return null;
			}

			if (name.IsEmpty())
			{
				return null;
			}

			return ServiceLocator.GetInstance(name);
		}

		private static object GetValue (Type type)
		{
			if (type == null)
			{
				return null;
			}

			return ServiceLocator.GetInstance(type);
		}

		private static void OnRegionNameChange (DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			string oldRegion = e.OldValue as string;
			string newRegion = e.NewValue as string;

			IRegionService regionService = ServiceLocator.GetInstance<IRegionService>();
			if (regionService == null)
			{
				LogLocator.LogWarning(typeof(RegionManager).Name, "No region service available while trying to assign region: Object={0}, OldRegion={1}, NewRegion={2}", obj?.GetType()?.Name ?? "[null]", oldRegion ?? "[null]", newRegion ?? "[null]");
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
