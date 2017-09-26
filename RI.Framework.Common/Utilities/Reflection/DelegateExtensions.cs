﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace RI.Framework.Utilities.Reflection
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="Delegate" /> type.
	/// </summary>
	public static class DelegateExtensions
	{
		/// <summary>
		/// Gets the full name of a delegates method.
		/// </summary>
		/// <param name="del">The delegate.</param>
		/// <returns>
		/// The full name of the delegated method.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The full name of a delegates method includes the namespace, type, and member, but not the assembly.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="del"/> is null.</exception>
		[SuppressMessage("ReSharper", "PossibleNullReferenceException")]
		public static string GetFullMethodName (this Delegate del)
		{
			if (del == null)
			{
				throw new ArgumentNullException(nameof(del));
			}

			return del.Method.DeclaringType.FullName + "." + del.Method.Name;
		}

		/// <summary>
		/// Gets the full name of a delegates target.
		/// </summary>
		/// <param name="del">The delegate.</param>
		/// <returns>
		/// The full name of the delegated target or null if no target is specified (e.g. for static methods).
		/// </returns>
		/// <remarks>
		/// <para>
		/// The full name of a delegates target includes the namespace, type, and member, but not the assembly.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="del"/> is null.</exception>
		public static string GetFullTargetName(this Delegate del)
		{
			if (del == null)
			{
				throw new ArgumentNullException(nameof(del));
			}

			Type type = del.Target?.GetType();
			if (type == null)
			{
				return null;
			}

			return type.FullName;
		}

		/// <summary>
		/// Gets the full name of a delegates target and method.
		/// </summary>
		/// <param name="del">The delegate.</param>
		/// <returns>
		/// The full name of the delegates target and method.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The full name of a delegates target and method consists of the target name, including namespace and type but not the assembly, followed by a &quot;@&quot;, followed by the method name. including namespace, type, and method but not the assembly.
		/// For static methods, only the method name, without &quot;@&quot; or a target name, is returned.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"><paramref name="del"/> is null.</exception>
		public static string GetFullName (this Delegate del)
		{
			if (del == null)
			{
				throw new ArgumentNullException(nameof(del));
			}

			string targetName = del.GetFullTargetName();
			string methodName = del.GetFullMethodName();
			return (targetName == null) ? methodName : (methodName + "@" + targetName);
		}
	}
}
