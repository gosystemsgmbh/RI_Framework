namespace RI.Framework.Composition.Model
{
	/// <summary>
	///     Used as a proxy in model-based importing (using <see cref="ImportPropertyAttribute" />) to hold multiple imported values.
	/// </summary>
	/// <remarks>
	///     <para>
	///         <see cref="ImportExtensions" /> must be used to access the actual imported values.
	///     </para>
	/// </remarks>
	public sealed class Import
	{
		#region Instance Constructor/Destructor

		internal Import ()
		{
		}

		#endregion




		#region Instance Fields

		internal object[] Instances = null;

		#endregion
	}
}
