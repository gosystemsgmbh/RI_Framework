﻿using System;
using System.Globalization;

using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Services.Resources
{
	/// <summary>
	///     Provides a centralized and global resource provider.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="ResourceLocator" /> is merely a convenience utility as it uses <see cref="ServiceLocator" /> to retrieve and use a <see cref="IResourceService" />.
	///     </para>
	/// </remarks>
	public static class ResourceLocator
	{
		#region Static Methods

		/// <summary>
		///     Gets a resource as its originally loaded type.
		/// </summary>
		/// <param name="name"> The name of the resource. </param>
		/// <returns>
		///     The resource value or null if the resource is not available.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		public static object GetRawValue (string name)
		{
			IResourceService resourceService = ServiceLocator.GetInstance<IResourceService>();
			return resourceService?.GetRawValue(name);
		}

		/// <summary>
		///     Gets a text resource as a string.
		/// </summary>
		/// <param name="name"> The name of the text resource. </param>
		/// <returns>
		///     The text string or null if the resource is not available.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		public static string GetText (string name)
		{
			return ResourceLocator.GetValue<string>(name);
		}

		/// <summary>
		///     Gets a text resource as a formatted string.
		/// </summary>
		/// <param name="name"> The name of the text resource to use as the format string. </param>
		/// <param name="formatProvider">The format provider or null to use <see cref="CultureInfo.CurrentCulture"/>.</param>
		/// <param name="args">The formatting arguments.</param>
		/// <returns>
		///     The formatted string or null if the resource is not available.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		public static string GetTextFormat(string name, IFormatProvider formatProvider, params object[] args)
		{
			string format = ResourceLocator.GetText(name);
			if (format == null)
			{
				return null;
			}

			return string.Format(formatProvider ?? CultureInfo.CurrentCulture, format, args);
		}

		/// <summary>
		///     Gets a resource as a value of a certain type.
		/// </summary>
		/// <typeparam name="T"> The resource type. </typeparam>
		/// <param name="name"> The name of the resource. </param>
		/// <returns>
		///     The resource value or the default value of <typeparamref name="T" /> if the resource is not available.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		/// <exception cref="InvalidTypeArgumentException"> The resource cannot be converted to type <typeparamref name="T" />. </exception>
		public static T GetValue <T> (string name)
			where T : class
		{
			IResourceService resourceService = ServiceLocator.GetInstance<IResourceService>();
			return resourceService?.GetValue<T>(name);
		}

		/// <summary>
		///     Gets a resource as a value of a certain type.
		/// </summary>
		/// <param name="type"> The resource type. </param>
		/// <param name="name"> The name of the resource. </param>
		/// <returns>
		///     The resource value or the default value of <paramref name="type" /> if the resource is not available.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> or <paramref name="type" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		/// <exception cref="InvalidTypeArgumentException"> The resource cannot be converted to type <paramref name="type" />. </exception>
		public static object GetValue (string name, Type type)
		{
			IResourceService resourceService = ServiceLocator.GetInstance<IResourceService>();
			return resourceService?.GetValue(name, type);
		}

		#endregion
	}
}