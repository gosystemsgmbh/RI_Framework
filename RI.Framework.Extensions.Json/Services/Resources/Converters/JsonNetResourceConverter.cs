using System;

using Newtonsoft.Json;

using RI.Framework.Composition.Model;
using RI.Framework.Services.Resources.Internals;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Services.Resources.Converters
{
    /// <summary>
    ///     Implements a resource converter which handles JSON-serialized resources.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The types supported by this resource converter are:
    ///         Any JSON-serializable object.
    ///     </para>
    ///     <note type="important">
    ///         Note that explicitly a type of <see cref="object" /> must be requested in order to trigger JSON-deserialization (which then must be casted in the required type explicitly).
    ///     </note>
    ///     <para>
    ///         See <see cref="IResourceConverter" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    [Export]
    public sealed class JsonNetResourceConverter : IResourceConverter
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="JsonNetResourceConverter" />.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Default JSON serialization settings are used.
        ///     </para>
        /// </remarks>
        public JsonNetResourceConverter ()
            : this(null)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="JsonNetResourceConverter" />.
        /// </summary>
        /// <param name="settings"> The used JSON serialization settings or null to use default settings. </param>
        public JsonNetResourceConverter (JsonSerializerSettings settings)
        {
            this.SyncRoot = new object();

            this.Settings = settings;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the used JSON serialization settings.
        /// </summary>
        /// <value>
        ///     The used JSON serialization settings or null if default settings are used.
        /// </value>
        public JsonSerializerSettings Settings { get; }

        #endregion




        #region Interface: IResourceConverter

        /// <inheritdoc />
        bool ISynchronizable.IsSynchronized => true;

        /// <inheritdoc />
        public object SyncRoot { get; }

        /// <inheritdoc />
        public bool CanConvert (Type sourceType, Type targetType)
        {
            if (sourceType == null)
            {
                throw new ArgumentNullException(nameof(sourceType));
            }

            if (targetType == null)
            {
                throw new ArgumentNullException(nameof(targetType));
            }

            if ((sourceType == typeof(string)) && (targetType == typeof(object)))
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public object Convert (Type type, object value)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if ((value is string) && (type == typeof(object)))
            {
                return JsonConvert.DeserializeObject((string)value, this.Settings);
            }

            throw new InvalidTypeArgumentException(nameof(value));
        }

        /// <inheritdoc />
        public ResourceLoadingInfo GetLoadingInfoFromFileExtension (string extension)
        {
            if (extension == null)
            {
                throw new ArgumentNullException(nameof(extension));
            }

            if (extension.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(extension));
            }

            extension = extension.ToUpperInvariant().Trim();

            if (extension == ".JSON")
            {
                return new ResourceLoadingInfo(ResourceLoadingType.Text, typeof(object));
            }

            return null;
        }

        #endregion
    }
}
