using System;
using System.Collections.Generic;

using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Services.Backup
{
    /// <summary>
    ///     Implements the inclusion description object.
    /// </summary>
    public class BackupInclusion : IEquatable<BackupInclusion>
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="BackupInclusion" />.
        /// </summary>
        /// <param name="id"> The ID of the inclusion. </param>
        /// <param name="resourceKey"> The resource key of the inclusion. Can be null. </param>
        /// <param name="supportsRestore"> Specifies whether the inclusion can be restored. </param>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="resourceKey" /> is an empty string. </exception>
        public BackupInclusion (Guid id, string resourceKey, bool supportsRestore)
        {
            if (resourceKey != null)
            {
                if (resourceKey.IsNullOrEmptyOrWhitespace())
                {
                    throw new EmptyStringArgumentException(nameof(resourceKey));
                }
            }

            this.Id = id;
            this.ResourceKey = resourceKey;
            this.SupportsRestore = supportsRestore;

            this.Tags = new Dictionary<string, string>(StringComparerEx.InvariantCultureIgnoreCase);
            this.Streams = new HashSet<Guid>();
        }

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override bool Equals (object obj) => this.Equals(obj as BackupInclusion);

        /// <inheritdoc />
        public override int GetHashCode () => this.Id.GetHashCode();

        #endregion




        #region Interface: IBackupInclusion

        /// <summary>
        ///     Gets the ID of the inclusion.
        /// </summary>
        /// <value>
        ///     The ID of the inclusion.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         The ID is used to uniquely identify an inclusion, allowing the ID to be serialized.
        ///     </para>
        /// </remarks>
        public Guid Id { get; }

        /// <summary>
        ///     Gets the resource key of the inclusion.
        /// </summary>
        /// <value>
        ///     The resource key of the inclusion or null if no resource key is specified.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         The resource key is used to associate a resource with the inclusion, e.g. a text resource for displaying the name of the inclusion to the user.
        ///     </para>
        /// </remarks>
        public string ResourceKey { get; }

        /// <summary>
        ///     Gets the set containing the stream IDs associated with this inclusion.
        /// </summary>
        /// <value>
        ///     The set containing the stream IDs associated with this inclusion.
        /// </value>
        public HashSet<Guid> Streams { get; }

        /// <summary>
        ///     Gets whether the inclusion can be restored.
        /// </summary>
        /// <value>
        ///     true if the inclusion can be restored, false otherwise.
        /// </value>
        public bool SupportsRestore { get; }

        /// <summary>
        ///     Gets the dictionary which can be used to store additional data about an inclusion.
        /// </summary>
        /// <value>
        ///     The dictionary which can be used to store additional data about an inclusion.
        /// </value>
        public Dictionary<string, string> Tags { get; }

        /// <inheritdoc />
        public bool Equals (BackupInclusion other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Id.Equals(other.Id);
        }

        #endregion
    }
}
