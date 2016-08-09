using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;




namespace RI.Framework.Utilities.Reflection
{
	/// <summary>
	///     Provides utility/extension methods for the <see cref="Assembly" /> type.
	/// </summary>
	public static class AssemblyExtensions
	{
		#region Static Methods

		/// <summary>
		///     Gets the company of an assembly.
		/// </summary>
		/// <param name="assembly"> The assembly. </param>
		/// <returns>
		///     The company of the assembly or null if the company could not be determined.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The <see cref="AssemblyCompanyAttribute" /> is used to determine the company of an assembly.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="assembly" /> is null. </exception>
		public static string GetCompany (this Assembly assembly)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException(nameof(assembly));
			}

			object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), true);

			if (attributes.Length == 0)
			{
				return null;
			}

			return ( (AssemblyCompanyAttribute)attributes[0] ).Company;
		}

		/// <summary>
		///     Gets the copyright of an assembly.
		/// </summary>
		/// <param name="assembly"> The assembly. </param>
		/// <returns>
		///     The copyright of the assembly or null if the copyright could not be determined.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The <see cref="AssemblyCopyrightAttribute" /> is used to determine the copyright of an assembly.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="assembly" /> is null. </exception>
		public static string GetCopyright (this Assembly assembly)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException(nameof(assembly));
			}

			object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true);

			if (attributes.Length == 0)
			{
				return null;
			}

			return ( (AssemblyCopyrightAttribute)attributes[0] ).Copyright;
		}

		/// <summary>
		///     Gets the description of an assembly.
		/// </summary>
		/// <param name="assembly"> The assembly. </param>
		/// <returns>
		///     The description of the assembly or null if the description could not be determined.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The <see cref="AssemblyDescriptionAttribute" /> is used to determine the description of an assembly.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="assembly" /> is null. </exception>
		public static string GetDescription (this Assembly assembly)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException(nameof(assembly));
			}

			object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), true);

			if (attributes.Length == 0)
			{
				return null;
			}

			return ( (AssemblyDescriptionAttribute)attributes[0] ).Description;
		}

		/// <summary>
		///     Gets the file of an assembly.
		/// </summary>
		/// <param name="assembly"> The assembly. </param>
		/// <returns>
		///     The file of the assembly or null if the file could not be determined.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="assembly" /> is null. </exception>
		public static string GetFile (this Assembly assembly)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException(nameof(assembly));
			}

#if PLATFORM_NET
			if (assembly.IsDynamic)
			{
				return null;
			}
#endif

			string location = null;

			try
			{
				location = assembly.Location;
			}
			catch
			{
				return null;
			}

			if (location.IsEmpty())
			{
				return null;
			}

			return location;
		}

		/// <summary>
		///     Gets a GUID associated with an assembly.
		/// </summary>
		/// <param name="assembly"> The assembly. </param>
		/// <param name="tryGuidAttribute"> Specifies whether <see cref="GuidAttribute" /> is considered for determining the GUID of the assembly. </param>
		/// <param name="ignoreVersion"> Specifies whether the assemblies version should be ignored for determining the GUID of the assembly. </param>
		/// <returns>
		///     The GUID of the assembly.
		/// </returns>
		/// <remarks>
		///     <para>
		///         If <paramref name="tryGuidAttribute" /> is true and the assembly has a <see cref="GuidAttribute" />, the GUID from that attribute is returned.
		///     </para>
		///     <para>
		///         If <paramref name="tryGuidAttribute" /> is false or <see cref="GuidAttribute" /> is not defined, the following is used to calculate a GUID:
		///         <see cref="AssemblyName.Name" /> when <paramref name="ignoreVersion" /> is true, <see cref="AssemblyName.FullName" /> otherwise.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="assembly" /> is null. </exception>
		public static Guid GetGuid (this Assembly assembly, bool tryGuidAttribute, bool ignoreVersion)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException(nameof(assembly));
			}

			object[] attributes = assembly.GetCustomAttributes(typeof(GuidAttribute), true);

			if (( attributes.Length > 0 ) && tryGuidAttribute)
			{
				Guid? guidCandidate = ( (GuidAttribute)attributes[0] ).Value.ToGuid();
				if (guidCandidate.HasValue)
				{
					return guidCandidate.Value;
				}
			}

			AssemblyName assemblyName = assembly.GetName();

			string guidInformationString = ignoreVersion ? assemblyName.Name : assemblyName.FullName;
			byte[] guidInformationBytes = Encoding.UTF8.GetBytes(guidInformationString);

			byte[] guidBytes = new byte[16];

			for (int i1 = 0; i1 < guidInformationBytes.Length; i1++)
			{
				guidBytes[i1 % 16] = (byte)( ( ( (int)guidBytes[i1 % 16] ) + ( (int)guidInformationBytes[i1] ) ) % 255 );
			}

			Guid guid = new Guid(guidBytes);
			return guid;
		}

		/// <summary>
		///     Gets the product name of an assembly.
		/// </summary>
		/// <param name="assembly"> The assembly. </param>
		/// <returns>
		///     The product name of the assembly or null if the product name could not be determined.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The <see cref="AssemblyProductAttribute" /> is used to determine the product name of an assembly.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="assembly" /> is null. </exception>
		public static string GetProduct (this Assembly assembly)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException(nameof(assembly));
			}

			object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), true);

			if (attributes.Length == 0)
			{
				return null;
			}

			return ( (AssemblyProductAttribute)attributes[0] ).Product;
		}

		/// <summary>
		///     Gets the title of an assembly.
		/// </summary>
		/// <param name="assembly"> The assembly. </param>
		/// <returns>
		///     The title of the assembly or null if the title could not be determined.
		/// </returns>
		/// <remarks>
		///     <para>
		///         The <see cref="AssemblyTitleAttribute" /> is used to determine the title of an assembly.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="assembly" /> is null. </exception>
		public static string GetTitle (this Assembly assembly)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException(nameof(assembly));
			}

			object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), true);

			if (attributes.Length == 0)
			{
				return null;
			}

			return ( (AssemblyTitleAttribute)attributes[0] ).Title;
		}

		/// <summary>
		///     Gets the version of an assembly.
		/// </summary>
		/// <param name="assembly"> The assembly. </param>
		/// <returns>
		///     The version of the assembly or null if the version could not be determined.
		/// </returns>
		/// <remarks>
		///     <para>
		///         One of the assembly versioning attributes is used in the following order (the first found is used) to determine the version of an assembly: <see cref="AssemblyVersionAttribute" />, <see cref="AssemblyFileVersionAttribute" />, <see cref="AssemblyInformationalVersionAttribute" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="assembly" /> is null. </exception>
		public static Version GetVersion (this Assembly assembly)
		{
			if (assembly == null)
			{
				throw ( new ArgumentNullException(nameof(assembly)) );
			}

			object[] attributes1 = assembly.GetCustomAttributes(typeof(AssemblyVersionAttribute), true);
			object[] attributes2 = assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true);
			object[] attributes3 = assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), true);

			if (attributes1.Length > 0)
			{
				return new Version(( (AssemblyVersionAttribute)attributes1[0] ).Version);
			}

			if (attributes2.Length > 0)
			{
				return new Version(( (AssemblyFileVersionAttribute)attributes2[0] ).Version);
			}

			if (attributes3.Length > 0)
			{
				return new Version(( (AssemblyInformationalVersionAttribute)attributes3[0] ).InformationalVersion);
			}

			return null;
		}

		#endregion
	}
}
