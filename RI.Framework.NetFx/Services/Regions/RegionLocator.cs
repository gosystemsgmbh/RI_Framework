using System;

using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Services.Regions
{
	/// <summary>
	///     Provides a centralized and global region provider.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="RegionLocator" /> is merely a convenience utility as it uses <see cref="ServiceLocator" /> to retrieve and use a <see cref="IRegionService" />.
	///     </para>
	/// </remarks>
	public static class RegionLocator
	{


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
		///         The elements instance is obtaines using <see cref="ServiceLocator" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> or <paramref name="element" /> is an empty string. </exception>
		/// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
		/// <exception cref="InvalidOperationException"> <paramref name="element" /> cannot be resolved or no region service is available. </exception>
		public static void Activate(string region, string element)
		{
			if (region == null)
			{
				throw new ArgumentNullException(nameof(region));
			}

			if (region.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(region));
			}

			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}

			if (element.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(element));
			}

			object value = RegionLocator.GetValue(element);
			if (value == null)
			{
				throw new InvalidOperationException();
			}

			RegionLocator.Activate(region, value);
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
		///         The elements instance is obtaines using <see cref="ServiceLocator" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> is an empty string. </exception>
		/// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
		/// <exception cref="InvalidOperationException"> <paramref name="element" /> cannot be resolved or no region service is available. </exception>
		public static void Activate(string region, Type element)
		{
			if (region == null)
			{
				throw new ArgumentNullException(nameof(region));
			}

			if (region.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(region));
			}

			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}

			object value = RegionLocator.GetValue(element);
			if (value == null)
			{
				throw new InvalidOperationException();
			}

			RegionLocator.Activate(region, value);
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
		public static void Activate(string region, object element)
		{
			if (region == null)
			{
				throw new ArgumentNullException(nameof(region));
			}

			if (region.IsEmptyOrWhitespace())
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
		///         The elements instance is obtaines using <see cref="ServiceLocator" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> or <paramref name="element" /> is an empty string. </exception>
		/// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
		/// <exception cref="InvalidOperationException"> <paramref name="element" /> cannot be resolved or no region service is available. </exception>
		public static void Add(string region, string element)
		{
			if (region == null)
			{
				throw new ArgumentNullException(nameof(region));
			}

			if (region.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(region));
			}

			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}

			if (element.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(element));
			}

			object value = RegionLocator.GetValue(element);
			if (value == null)
			{
				throw new InvalidOperationException();
			}

			RegionLocator.Add(region, value);
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
		///         The elements instance is obtaines using <see cref="ServiceLocator" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> is an empty string. </exception>
		/// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
		/// <exception cref="InvalidOperationException"> <paramref name="element" /> cannot be resolved or no region service is available. </exception>
		public static void Add(string region, Type element)
		{
			if (region == null)
			{
				throw new ArgumentNullException(nameof(region));
			}

			if (region.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(region));
			}

			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}

			object value = RegionLocator.GetValue(element);
			if (value == null)
			{
				throw new InvalidOperationException();
			}

			RegionLocator.Add(region, value);
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
		public static void Add(string region, object element)
		{
			if (region == null)
			{
				throw new ArgumentNullException(nameof(region));
			}

			if (region.IsEmptyOrWhitespace())
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
		///         The elements instance is obtaines using <see cref="ServiceLocator" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> or <paramref name="element" /> is an empty string. </exception>
		/// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
		/// <exception cref="InvalidOperationException"> <paramref name="element" /> cannot be resolved or no region service is available. </exception>
		public static bool CanNavigate(string region, string element)
		{
			if (region == null)
			{
				throw new ArgumentNullException(nameof(region));
			}

			if (region.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(region));
			}

			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}

			if (element.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(element));
			}

			object value = RegionLocator.GetValue(element);
			if (value == null)
			{
				throw new InvalidOperationException();
			}

			return RegionLocator.CanNavigate(region, value);
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
		///         The elements instance is obtaines using <see cref="ServiceLocator" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> is an empty string. </exception>
		/// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
		/// <exception cref="InvalidOperationException"> <paramref name="element" /> cannot be resolved or no region service is available. </exception>
		public static bool CanNavigate(string region, Type element)
		{
			if (region == null)
			{
				throw new ArgumentNullException(nameof(region));
			}

			if (region.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(region));
			}

			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}

			object value = RegionLocator.GetValue(element);
			if (value == null)
			{
				throw new InvalidOperationException();
			}

			return RegionLocator.CanNavigate(region, value);
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
		public static bool CanNavigate(string region, object element)
		{
			if (region == null)
			{
				throw new ArgumentNullException(nameof(region));
			}

			if (region.IsEmptyOrWhitespace())
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
		public static void Clear(string region)
		{
			if (region == null)
			{
				throw new ArgumentNullException(nameof(region));
			}

			if (region.IsEmptyOrWhitespace())
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
		///         The elements instance is obtaines using <see cref="ServiceLocator" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> or <paramref name="element" /> is an empty string. </exception>
		/// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
		/// <exception cref="InvalidOperationException"> <paramref name="element" /> cannot be resolved or no region service is available. </exception>
		public static void Deactivate(string region, string element)
		{
			if (region == null)
			{
				throw new ArgumentNullException(nameof(region));
			}

			if (region.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(region));
			}

			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}

			if (element.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(element));
			}

			object value = RegionLocator.GetValue(element);
			if (value == null)
			{
				throw new InvalidOperationException();
			}

			RegionLocator.Deactivate(region, value);
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
		///         The elements instance is obtaines using <see cref="ServiceLocator" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> is an empty string. </exception>
		/// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
		/// <exception cref="InvalidOperationException"> <paramref name="element" /> cannot be resolved or no region service is available. </exception>
		public static void Deactivate(string region, Type element)
		{
			if (region == null)
			{
				throw new ArgumentNullException(nameof(region));
			}

			if (region.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(region));
			}

			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}

			object value = RegionLocator.GetValue(element);
			if (value == null)
			{
				throw new InvalidOperationException();
			}

			RegionLocator.Deactivate(region, value);
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
		public static void Deactivate(string region, object element)
		{
			if (region == null)
			{
				throw new ArgumentNullException(nameof(region));
			}

			if (region.IsEmptyOrWhitespace())
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
		public static void DeactivateAll(string region)
		{
			if (region == null)
			{
				throw new ArgumentNullException(nameof(region));
			}

			if (region.IsEmptyOrWhitespace())
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
		///         The elements instance is obtaines using <see cref="ServiceLocator" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> or <paramref name="element" /> is an empty string. </exception>
		/// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
		/// <exception cref="InvalidOperationException"> <paramref name="element" /> cannot be resolved or no region service is available. </exception>
		public static bool Navigate(string region, string element)
		{
			if (region == null)
			{
				throw new ArgumentNullException(nameof(region));
			}

			if (region.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(region));
			}

			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}

			if (element.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(element));
			}

			object value = RegionLocator.GetValue(element);
			if (value == null)
			{
				throw new InvalidOperationException();
			}

			return RegionLocator.Navigate(region, value);
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
		///         The elements instance is obtaines using <see cref="ServiceLocator" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> is an empty string. </exception>
		/// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
		/// <exception cref="InvalidOperationException"> <paramref name="element" /> cannot be resolved or no region service is available. </exception>
		public static bool Navigate(string region, Type element)
		{
			if (region == null)
			{
				throw new ArgumentNullException(nameof(region));
			}

			if (region.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(region));
			}

			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}

			object value = RegionLocator.GetValue(element);
			if (value == null)
			{
				throw new InvalidOperationException();
			}

			return RegionLocator.Navigate(region, value);
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
		public static bool Navigate(string region, object element)
		{
			if (region == null)
			{
				throw new ArgumentNullException(nameof(region));
			}

			if (region.IsEmptyOrWhitespace())
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
		///         The elements instance is obtaines using <see cref="ServiceLocator" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> or <paramref name="element" /> is an empty string. </exception>
		/// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
		/// <exception cref="InvalidOperationException"> <paramref name="element" /> cannot be resolved or no region service is available. </exception>
		public static void Remove(string region, string element)
		{
			if (region == null)
			{
				throw new ArgumentNullException(nameof(region));
			}

			if (region.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(region));
			}

			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}

			if (element.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(element));
			}

			object value = RegionLocator.GetValue(element);
			if (value == null)
			{
				throw new InvalidOperationException();
			}

			RegionLocator.Remove(region, value);
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
		///         The elements instance is obtaines using <see cref="ServiceLocator" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="region" /> or <paramref name="element" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="region" /> is an empty string. </exception>
		/// <exception cref="RegionNotFoundException"> The region specified by <paramref name="region" /> does not exist. </exception>
		/// <exception cref="InvalidOperationException"> <paramref name="element" /> cannot be resolved or no region service is available. </exception>
		public static void Remove(string region, Type element)
		{
			if (region == null)
			{
				throw new ArgumentNullException(nameof(region));
			}

			if (region.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(region));
			}

			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}

			object value = RegionLocator.GetValue(element);
			if (value == null)
			{
				throw new InvalidOperationException();
			}

			RegionLocator.Remove(region, value);
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
		public static void Remove(string region, object element)
		{
			if (region == null)
			{
				throw new ArgumentNullException(nameof(region));
			}

			if (region.IsEmptyOrWhitespace())
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


		private static object GetValue(string name)
		{
			if (name == null)
			{
				return null;
			}

			if (name.IsEmptyOrWhitespace())
			{
				return null;
			}

			return ServiceLocator.GetInstance(name);
		}

		private static object GetValue(Type type)
		{
			if (type == null)
			{
				return null;
			}

			return ServiceLocator.GetInstance(type);
		}
	}
}