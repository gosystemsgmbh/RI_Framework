using System;

using RI.Framework.Services.Resources.Sources;




namespace RI.Framework.Services.Resources
{
	/// <summary>
	///     Describes the loading type of a raw resource value.
	/// </summary>
	[Serializable]
	public enum ResourceLoadingType
	{
		/// <summary>
		///     The loading type is unknown by the resource converter and must be determined by the <see cref="IResourceSet" /> or <see cref="IResourceSource" /> itself.
		/// </summary>
		Unknown = 0,

		/// <summary>
		///     The resource is to be loaded as an array of <see cref="byte" />s.
		/// </summary>
		Binary = 1,

		/// <summary>
		///     The resource is to be loaded as a <see cref="string" />.
		/// </summary>
		Text = 2,
	}
}
