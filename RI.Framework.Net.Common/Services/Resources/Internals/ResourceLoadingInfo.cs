using System;

using RI.Framework.Services.Resources.Converters;
using RI.Framework.Services.Resources.Sources;




namespace RI.Framework.Services.Resources.Internals
{
    /// <summary>
    ///     Describes, from the perspective of a <see cref="IResourceConverter" />, how a raw resource value is to be loaded by its associated <see cref="IResourceSet" /> or <see cref="IResourceSource" />.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public sealed class ResourceLoadingInfo
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="ResourceLoadingInfo" />.
		/// </summary>
		/// <param name="loadingType"> The loading type of the raw resource value. </param>
		/// <param name="resourceType"> The resource type supported by a resource converter. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="resourceType" /> is null when <paramref name="loadingType" /> is not <see cref="ResourceLoadingType.Unknown" /> </exception>
		public ResourceLoadingInfo (ResourceLoadingType loadingType, Type resourceType)
		{
			if ((loadingType != ResourceLoadingType.Unknown) && (resourceType == null))
			{
				throw new ArgumentNullException(nameof(resourceType));
			}

			this.LoadingType = loadingType;
			this.ResourceType = resourceType;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the loading type of the raw resource value.
		/// </summary>
		/// <value>
		///     The loading type of the raw resource value.
		/// </value>
		public ResourceLoadingType LoadingType { get; }

		/// <summary>
		///     Gets the resource type supported by a resource converter.
		/// </summary>
		/// <value>
		///     The resource type supported by a resource converter.
		/// </value>
		public Type ResourceType { get; }

		#endregion
	}
}
