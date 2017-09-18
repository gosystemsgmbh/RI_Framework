using System;
using System.Collections.Generic;

using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Services.Backup
{
	/// <summary>
	///     Implements a default inclusion type.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="IBackupInclusion" /> for more details.
	///     </para>
	/// </remarks>
	public class BackupInclusion : IBackupInclusion
	{
		/// <summary>
		/// Creates a new instance of <see cref="BackupInclusion"/>.
		/// </summary>
		/// <param name="id">The ID of the inclusion.</param>
		/// <param name="resourceKey">The resource key of the inclusion. Can be null.</param>
		/// <param name="supportsRestore">Specifies whether the inclusion can be restored.</param>
		/// <exception cref="EmptyStringArgumentException"><paramref name="resourceKey"/> is an empty string.</exception>
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

		/// <inheritdoc />
		public bool Equals (IBackupInclusion other)
		{
			if (other == null)
			{
				return false;
			}

			BackupInclusion other2 = other as BackupInclusion;
			if (other2 == null)
			{
				return false;
			}

			return this.Id.Equals(other2.Id);
		}

		/// <inheritdoc />
		public override int GetHashCode () => this.Id.GetHashCode();

		/// <inheritdoc />
		public override bool Equals (object obj) => this.Equals(obj as IBackupInclusion);

		/// <inheritdoc />
		public Guid Id { get; }

		/// <inheritdoc />
		public string ResourceKey { get; }

		/// <inheritdoc />
		public bool SupportsRestore { get; }

		/// <inheritdoc />
		public IDictionary<string, string> Tags { get; }

		/// <inheritdoc />
		public ISet<Guid> Streams { get; }
	}
}
