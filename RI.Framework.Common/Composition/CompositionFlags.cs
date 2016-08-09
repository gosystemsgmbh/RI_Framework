using System;
using System.Diagnostics.CodeAnalysis;

using RI.Framework.Composition.Model;




namespace RI.Framework.Composition
{
	/// <summary>
	///     Used to define the type of composition to perform on model-based imports by a <see cref="CompositionContainer" />.
	/// </summary>
	[Serializable]
	[Flags]
	[SuppressMessage ("ReSharper", "UnusedMember.Global")]
	public enum CompositionFlags
	{
		/// <summary>
		///     No composition.
		/// </summary>
		None = 0x00,

		/// <summary>
		///     Internally used by <see cref="CompositionContainer" /> but same as <see cref="All" /> when used by your code.
		/// </summary>
		Constructing = 0x01,

		/// <summary>
		///     Imports which have a value of null are resolved.
		/// </summary>
		Missing = 0x02,

		/// <summary>
		///     Imports which are recomposable (<see cref="ImportPropertyAttribute" />.<see cref="ImportPropertyAttribute.Recomposable" /> is true) are resolved (existing values, if any, are overwritten).
		/// </summary>
		Recomposable = 0x04,

		/// <summary>
		///     Imports which have non-null value are resolved (existing values are overwritten).
		/// </summary>
		Composed = 0x08,

		/// <summary>
		///     Combination of <see cref="Missing" /> and <see cref="Recomposable" />.
		/// </summary>
		Normal = CompositionFlags.Missing | CompositionFlags.Recomposable,

		/// <summary>
		///     Combination of <see cref="Missing" />, <see cref="Recomposable" />, and <see cref="Composed" />.
		/// </summary>
		All = CompositionFlags.Missing | CompositionFlags.Recomposable | CompositionFlags.Composed,
	}
}
