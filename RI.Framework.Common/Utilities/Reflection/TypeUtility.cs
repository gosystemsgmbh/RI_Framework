using System;
using System.Collections.Generic;
using System.Reflection;

using RI.Framework.Collections.DirectLinq;
using RI.Framework.Utilities.Exceptions;

namespace RI.Framework.Utilities.Reflection
{
	/// <summary>
	///     Provides a utility for types.
	/// </summary>
	public static class TypeUtility
	{
		/// <summary>
		/// Tries to find the type in all assemblies of the current application domain.
		/// </summary>
		/// <param name="typeToFind">The name of the type to find.</param>
		/// <param name="predicate">The optional predicate to detect the desired type or null if the default detection should be used.</param>
		/// <returns>
		/// The found type or null if the type was not found.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The default detection, if <paramref name="predicate"/> is null, is to compare <see cref="MemberInfo.Name"/> case-sensitive with <paramref name="typeToFind"/>.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="typeToFind"/> is null.</exception>
		/// <exception cref="EmptyStringArgumentException"><paramref name="typeToFind"/> is an empty string.</exception>
		public static Type FindTypeInAppDomain (string typeToFind, Func<string,Type,bool> predicate)
		{
			if (typeToFind == null)
			{
				throw new ArgumentNullException(nameof(typeToFind));
			}

			if (typeToFind.IsNullOrEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(typeToFind));
			}

			predicate = predicate ?? ((s, t) => string.Equals(s, t.Name, StringComparison.Ordinal));

			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			List<Type> types = assemblies.SelectMany(x => x.GetTypes());
			foreach (Type type in types)
			{
				if (predicate(typeToFind, type))
				{
					return type;
				}
			}

			return null;
		}
	}
}
