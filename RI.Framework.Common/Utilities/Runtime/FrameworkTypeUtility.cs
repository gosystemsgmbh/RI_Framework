using System;
using System.Reflection;

using RI.Framework.IO.Paths;




namespace RI.Framework.Utilities.Runtime
{
	/// <summary>
	///     Provides a utility to determine whether a type or assembly is part of the framework.
	/// </summary>
	public static class FrameworkTypeUtility
	{
		#region Constants

		private const string FrameworkNamespacePrefix = nameof(RI) + "." + nameof(Framework);

		#endregion




		#region Static Methods

		/// <summary>
		///     Determines whether an assembly is part of the framework.
		/// </summary>
		/// <param name="assembly"> The assembly. </param>
		/// <returns>
		///     true if the assembly is part of the framework, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="assembly" /> is null. </exception>
		public static bool IsFrameworkAssembly (Assembly assembly)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException(nameof(assembly));
			}

			return assembly.GetName().Name.StartsWith(FrameworkTypeUtility.FrameworkNamespacePrefix, StringComparison.Ordinal);
		}

		/// <summary>
		///     Determines whether a file is part of the framework.
		/// </summary>
		/// <param name="file"> The file. </param>
		/// <returns>
		///     true if the file is part of the framework, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="file" /> is null. </exception>
		public static bool IsFrameworkFile (FilePath file)
		{
			if (file == null)
			{
				throw new ArgumentNullException(nameof(file));
			}

			return file.FileName.StartsWith(FrameworkTypeUtility.FrameworkNamespacePrefix, StringComparison.Ordinal);
		}

		/// <summary>
		///     Determines whether a type is part of the framework.
		/// </summary>
		/// <param name="type"> The type. </param>
		/// <returns>
		///     true if the type is part of the framework, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="type" /> is null. </exception>
		public static bool IsFrameworkType (Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			return type.Namespace?.StartsWith(FrameworkTypeUtility.FrameworkNamespacePrefix, StringComparison.Ordinal) ?? false;
		}

		#endregion
	}
}
