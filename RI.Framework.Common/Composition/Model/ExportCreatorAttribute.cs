using System;
using System.Diagnostics.CodeAnalysis;




namespace RI.Framework.Composition.Model
{
	/// <summary>
	///     Defines the static method of an exported type (called &quot;export method&quot;) which is used for creating an instance of that type.
	/// </summary>
	/// <remarks>
	///     <para>
	///         An export method can be used in cases where an instance of an exported type is not obtained through its constructor but through a custom procedure which is implemented in a static method.
	///         This tells the <see cref="CompositionContainer" /> which export method to use when creating an instance of the type during resolving of imports.
	///         The export method must be a static method in the type which is to be exported or in one of its base classes.
	///     </para>
	///     <para>
	///         An export method has at least one parameter which is a <see cref="Type" /> and specifies the exact type to be instantiated.
	///         Aditional parameters are possible but the type to be instantiated must always be the first parameter.
	///         If additional parameters are used, the parameter values are resolved similar as resolving manual imports, using the default names of the parameter types.
	///         See <see cref="CompositionContainer" /> for more details about manual imports and the default name of a type.
	///     </para>
	/// </remarks>
	[AttributeUsage (AttributeTargets.Method)]
	[SuppressMessage ("ReSharper", "MemberCanBeInternal")]
	public sealed class ExportCreatorAttribute : Attribute
	{
	}
}
