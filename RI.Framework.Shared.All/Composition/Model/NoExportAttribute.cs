using System;

using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Composition.Model
{
	/// <summary>
	///     Defines that, for model-based exporting (using <see cref="CompositionCatalog" />), a type is not exported.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="NoExportAttribute"/> can be helpfule in cases where a type inherits an <see cref="ExportAttribute"/> (e.g. through an interface) but should not be exported through model-based exporting.
	///     </para>
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
	public sealed class NoExportAttribute : Attribute
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Specifies a type to be no longer exported.
		/// </summary>
		/// <remarks>
		///     <para>
		///         See <see cref="CompositionContainer" /> for more details about exporting.
		///     </para>
		/// </remarks>
		public NoExportAttribute()
		{
		}

		#endregion
	}
}
