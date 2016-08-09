using System;
using System.Diagnostics.CodeAnalysis;




namespace RI.Framework.Composition.Model
{
	/// <summary>
	///     Defines the export constructor of an exported type.
	/// </summary>
	/// <remarks>
	///     <para>
	///         An export constructor is required to be defined when an exported type has either multiple constructors or no default constructor.
	///         This tells the <see cref="CompositionContainer" /> which constructor to use when creating an instance of the type during resolving of imports.
	///     </para>
	///     <para>
	///         An export constructor can have parameters.
	///         If so, the parameter values are resolved similar as resolving manual imports, using the default names of the parameter types.
	///         See <see cref="CompositionContainer" /> for more details about manual imports and the default name of a type.
	///     </para>
	/// </remarks>
	[AttributeUsage (AttributeTargets.Constructor)]
	[SuppressMessage ("ReSharper", "MemberCanBeInternal")]
	public sealed class ExportConstructorAttribute : Attribute
	{
	}
}
