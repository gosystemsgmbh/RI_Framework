using System;
using System.Collections.Generic;

using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Data
{
	/// <summary>
	///     Represents validation errors of an entity.
	/// </summary>
	/// <remarks>
	///     <para>
	///         Valiation errors are returned by <see cref="IRepositorySet{T}.Validate" />.
	///     </para>
	/// </remarks>
	public class RepositoryErrors
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates anew instance of <see cref="RepositoryErrors" />.
		/// </summary>
		/// <remarks>
		///     <para>
		///         <see cref="StringComparer.InvariantCultureIgnoreCase" /> is used for the property names and <see cref="StringComparer.CurrentCultureIgnoreCase" /> for the errors.
		///     </para>
		/// </remarks>
		public RepositoryErrors ()
			: this(null, null)
		{
		}

		/// <summary>
		///     Creates anew instance of <see cref="RepositoryErrors" />.
		/// </summary>
		/// <param name="propertyNameComparer"> The property name comparer used with the <see cref="PropertyErrors" /> dictionary or null to use <see cref="StringComparer.InvariantCultureIgnoreCase" />. </param>
		/// <param name="errorComparer"> The error comparer used with the <see cref="EntityErrors" /> and <see cref="PropertyErrors" /> sets or null to use <see cref="StringComparer.CurrentCultureIgnoreCase" />. </param>
		public RepositoryErrors (IEqualityComparer<string> propertyNameComparer, IEqualityComparer<string> errorComparer)
		{
			this.PropertyNameComparer = propertyNameComparer ?? StringComparer.InvariantCultureIgnoreCase;
			this.ErrorComparer = errorComparer ?? StringComparer.CurrentCultureIgnoreCase;

			this.EntityErrors = new HashSet<string>(this.ErrorComparer);
			this.PropertyErrors = new Dictionary<string, HashSet<string>>(this.PropertyNameComparer);
		}

		#endregion




		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the list of entity-level validation errors which apply to the entity as a whole.
		/// </summary>
		/// <value>
		///     The list of entity-level validation errors.
		/// </value>
		public HashSet<string> EntityErrors { get; private set; }

		/// <summary>
		///     Gets the dictionary of property-level validation errors which apply to specific properties of the entity.
		/// </summary>
		/// <value>
		///     The dictionary of property-level validation errors.
		/// </value>
		/// <remarks>
		///     <para>
		///         The keys of the dictionary are the property names and the values are the lists of validation errors associated with the corresponding property.
		///     </para>
		///     <para>
		///         The dictionary uses <see cref="StringComparer.CurrentCultureIgnoreCase" /> for managing the property keys.
		///     </para>
		/// </remarks>
		public Dictionary<string, HashSet<string>> PropertyErrors { get; private set; }

		/// <summary>
		///     Gets the error comparer used with the <see cref="EntityErrors" /> and <see cref="PropertyErrors" /> sets.
		/// </summary>
		/// <value>
		///     The error comparer used with the <see cref="EntityErrors" /> and <see cref="PropertyErrors" /> sets.
		/// </value>
		protected IEqualityComparer<string> ErrorComparer { get; private set; }

		/// <summary>
		///     Gets the property name comparer used with the <see cref="PropertyErrors" /> dictionary.
		/// </summary>
		/// <value>
		///     The property name comparer used with the <see cref="PropertyErrors" /> dictionary.
		/// </value>
		protected IEqualityComparer<string> PropertyNameComparer { get; private set; }

		#endregion




		#region Instance Methods

		/// <summary>
		///     Adds another entity-level error.
		/// </summary>
		/// <param name="error"> The error to add. </param>
		/// <remarks>
		///     <para>
		///         If <paramref name="error" /> is null or an empty string, nothing is added.
		///     </para>
		/// </remarks>
		public void AddEntityError (string error)
		{
			if (error.IsNullOrEmpty())
			{
				return;
			}

			this.EntityErrors.Add(error);
		}

		/// <summary>
		///     Adds another property-level error.
		/// </summary>
		/// <param name="property"> The property the error belongs to. </param>
		/// <param name="error"> The error to add. </param>
		/// <remarks>
		///     <para>
		///         If <paramref name="error" /> is null or an empty string, nothing is added.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="property" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="property" /> is an empty string. </exception>
		public void AddPropertyError (string property, string error)
		{
			if (property == null)
			{
				throw new ArgumentNullException(nameof(property));
			}

			if (property.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(property));
			}

			if (error.IsNullOrEmpty())
			{
				return;
			}

			if (!this.PropertyErrors.ContainsKey(property))
			{
				this.PropertyErrors.Add(property, new HashSet<string>(this.ErrorComparer));
			}

			this.PropertyErrors[property].Add(error);
		}

		#endregion
	}
}
