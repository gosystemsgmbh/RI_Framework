using System;
using System.Collections.Generic;




namespace RI.Framework.Utilities.Reflection
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="Type" /> type.
	/// </summary>
	public static class TypeExtensions
	{
		#region Static Methods

		/// <summary>
		///     Gets all types the specified type inherits from.
		/// </summary>
		/// <param name="type"> The type of which the inheritance list should be returned. </param>
		/// <param name="includeSelf"> Specifies whether <paramref name="type" /> is also included in the returned inheritance list. </param>
		/// <returns>
		///     The list with all types <paramref name="type" /> inherits from.
		///     The list is empty if the type is <see cref="object" /> and <paramref name="includeSelf" /> is false.
		///     The list starts with the root type of the inheritance, which is always <see cref="object" />.
		/// </returns>
		/// <remarks>
		///     <note type="note">
		///         The returned inheritance list does only contain base classes but not interfaces.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="type" /> is null. </exception>
		public static List<Type> GetInheritance (this Type type, bool includeSelf)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			List<Type> types = new List<Type>();

			if (includeSelf)
			{
				types.Add(type);
			}

			while (type.BaseType != null)
			{
				type = type.BaseType;
				types.Add(type);
			}

			types.Reverse();

			return types;
		}

		#endregion
	}
}
