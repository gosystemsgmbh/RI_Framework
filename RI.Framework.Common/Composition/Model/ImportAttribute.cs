using System;

using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Composition.Model
{
	/// <summary>
	///     Defines a model-based import of a specified name for a property.
	/// </summary>
	/// <remarks>
	///     <para>
	///         In cases where a single value needs to be imported, the property type (of the property <see cref="ImportAttribute" /> is applied to) can be of the imported type.
	///     </para>
	///     <para>
	///         In cases where multiple values need to be imported, the property type (of the property <see cref="ImportAttribute" /> is applied to) must be <see cref="Import" /> and <see cref="ImportExtensions" /> must be used to access the actual imported values.
	///     </para>
	/// </remarks>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
	public sealed class ImportAttribute : Attribute
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Imports the property value by using the property types default name for resolving.
		/// </summary>
		/// <remarks>
		///     <para>
		///         See <see cref="CompositionContainer" /> for more details about the default name of a type.
		///     </para>
		/// </remarks>
		public ImportAttribute ()
		{
			this.Name = null;
			this.Recomposable = true;
		}

		/// <summary>
		///     Imports the property value by using the specified types default name for resolving.
		/// </summary>
		/// <param name="type"> The type whose default name is used for resolving. </param>
		/// <remarks>
		///     <para>
		///         See <see cref="CompositionContainer" /> for more details about the default name of a type.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="type" /> is null. </exception>
		public ImportAttribute (Type type)
			: this()
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			this.Name = CompositionContainer.GetNameOfType(type);
		}

		/// <summary>
		///     Imports the property value by using the specified name for resolving.
		/// </summary>
		/// <param name="name"> The name which is used for resolving. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="name" /> is an empty string. </exception>
		public ImportAttribute (string name)
			: this()
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			this.Name = name;
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the name which is used to resolve the value for the decorated property.
		/// </summary>
		/// <value>
		///     The name which is used to resolve the value for the decorated property.
		/// </value>
		public string Name { get; private set; }

		/// <summary>
		///     Gets or sets whether the value for the decorated property is reimported automatically when the exports of the corresponding name changed.
		/// </summary>
		/// <value>
		///     true if the decorated property is automatically updated, false otherwise.
		/// </value>
		/// <remarks>
		///     <para>
		///         The default value is true.
		///     </para>
		///     <note type="note">
		///         Automatic reimport is only performed if the object, to which the decorated property belongs, is itself an export of the corresponding <see cref="CompositionContainer" />.
		///     </note>
		/// </remarks>
		public bool Recomposable { get; set; }

		#endregion
	}
}
