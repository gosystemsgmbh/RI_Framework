using System;
using System.Diagnostics.CodeAnalysis;

using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Composition
{
	/// <summary>
	///     Used to encapsulate a single export managed by a <see cref="CompositionCatalog" />.
	/// </summary>
	/// <remarks>
	///     <para>
	///         An export can only have either an assigned type (using the <see cref="Type" /> property) or an object (using the <see cref="Value" /> property), but not both.
	///         Which one is used depends on the <see cref="CompositionCatalog" /> which is managing the export.
	///     </para>
	/// </remarks>
	[SuppressMessage ("ReSharper", "MemberCanBeInternal")]
	public sealed class CompositionCatalogItem
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="CompositionCatalogItem" />.
		/// </summary>
		/// <param name="name"> The name under which the export is exported. </param>
		/// <param name="type"> The type which is exported. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> or <paramref name="type" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		public CompositionCatalogItem (string name, Type type)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			this.Name = name;
			this.Type = type;
			this.Value = null;
		}

		/// <summary>
		///     Creates a new instance of <see cref="CompositionCatalogItem" />.
		/// </summary>
		/// <param name="name"> The name under which the export is exported. </param>
		/// <param name="value"> The object which is exported. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> or <paramref name="value" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		public CompositionCatalogItem (string name, object value)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}

			this.Name = name;
			this.Type = null;
			this.Value = value;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the name under which the export is exported.
		/// </summary>
		/// <value>
		///     The name under which the export is exported.
		/// </value>
		public string Name { get; private set; }

		/// <summary>
		///     Gets the type which is exported.
		/// </summary>
		/// <value>
		///     The type which is exported or null if an object is exported instead.
		/// </value>
		public Type Type { get; private set; }

		/// <summary>
		///     Gets the object which is exported.
		/// </summary>
		/// <value>
		///     The object which is exported or null if a type is exported instead.
		/// </value>
		public object Value { get; private set; }

		#endregion
	}
}
