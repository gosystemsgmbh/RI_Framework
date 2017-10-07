using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using RI.Framework.Collections;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Data.Repository
{
	/// <summary>
	///     Represents validation errors of an entity.
	/// </summary>
	/// <remarks>
	///     <para>
	///         For equality comparison, <see cref="StringComparerEx.Ordinal" /> is used for the property names and <see cref="StringComparerEx.OrdinalIgnoreCase" /> for the errors.
	///     </para>
	///     <note type="note">
	///         <see cref="RepositorySetErrors" /> is serializable, using <see cref="ISerializable" />.
	///         Therefore, to serialize and deserialize values in types inheriting from <see cref="RepositorySetErrors" />, you must use <see cref="GetObjectData" /> and <see cref="RepositorySetErrors(SerializationInfo,StreamingContext)" />.
	///     </note>
	/// </remarks>
	[Serializable]
	public class RepositorySetErrors : ISerializable
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates anew instance of <see cref="RepositorySetErrors" />.
		/// </summary>
		public RepositorySetErrors ()
		{
			this.EntityErrors = new HashSet<string>(StringComparerEx.Ordinal);
			this.PropertyErrors = new Dictionary<string, HashSet<string>>(StringComparerEx.OrdinalIgnoreCase);
		}

		/// <summary>
		///     Creates a new instance of <see cref="RepositorySetErrors" />.
		/// </summary>
		/// <param name="info"> The serialization data. </param>
		/// <param name="context"> The type of the source of the serialization data. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="info" /> is null. </exception>
		public RepositorySetErrors (SerializationInfo info, StreamingContext context)
			: this()
		{
			if (info == null)
			{
				throw new ArgumentNullException(nameof(info));
			}

			string[] entityErrors = (string[])info.GetValue(nameof(this.EntityErrors), typeof(string[]));

			Dictionary<string, string[]> propertyErrors = (Dictionary<string, string[]>)info.GetValue(nameof(this.PropertyErrors), typeof(Dictionary<string, string[]>));

			entityErrors?.ForEach(x => this.AddEntityError(x));
			propertyErrors?.ForEach(x => x.Value.ForEach(y => this.AddPropertyError(x.Key, y)));
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
		///         The dictionary uses <see cref="StringComparerEx.CurrentCultureIgnoreCase" /> for managing the property keys.
		///     </para>
		/// </remarks>
		public Dictionary<string, HashSet<string>> PropertyErrors { get; private set; }

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
			if (error.IsNullOrEmptyOrWhitespace())
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

			if (property.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(property));
			}

			if (error.IsNullOrEmptyOrWhitespace())
			{
				return;
			}

			if (!this.PropertyErrors.ContainsKey(property))
			{
				this.PropertyErrors.Add(property, new HashSet<string>(this.PropertyErrors.Comparer));
			}

			this.PropertyErrors[property].Add(error);
		}

		/// <summary>
		///     Gets a list with all validation errors.
		/// </summary>
		/// <returns>
		///     The list with all validation errors or null if no validation errors are defined.
		/// </returns>
		public List<string> ToErrorList ()
		{
			List<string> list = new List<string>();

			foreach (string error in this.EntityErrors)
			{
				list.Add(error);
			}

			foreach (KeyValuePair<string, HashSet<string>> propertyError in this.PropertyErrors)
			{
				foreach (string error in propertyError.Value)
				{
					list.Add(error);
				}
			}

			return list.Count == 0 ? null : list;
		}

		/// <summary>
		///     Converts all the validation errors into one string for display.
		/// </summary>
		/// <returns>
		///     The string containing all validation errors or null if no validation errors are defined.
		/// </returns>
		/// <remarks>
		///     <para>
		///         Each validation error is separated by a new line.
		///     </para>
		/// </remarks>
		public string ToErrorString ()
		{
			return this.ToErrorString(null);
		}

		/// <summary>
		///     Converts all the validation errors into one string for display.
		/// </summary>
		/// <param name="separator"> The separator character to use between each validation error. </param>
		/// <returns>
		///     The string containing all validation errors or null if no validation errors are defined.
		/// </returns>
		public string ToErrorString (char separator)
		{
			return this.ToErrorString(new string(separator, 1));
		}

		/// <summary>
		///     Converts all the validation errors into one string for display.
		/// </summary>
		/// <param name="separator"> The separator string to use between each validation error or null if a new line is to be used as a separator. </param>
		/// <returns>
		///     The string containing all validation errors or null if no validation errors are defined.
		/// </returns>
		public string ToErrorString (string separator)
		{
			return this.ToErrorList()?.Join(separator ?? Environment.NewLine);
		}

		#endregion




		#region Virtuals

		/// <inheritdoc cref="ISerializable.GetObjectData" />
		protected virtual void GetObjectData (SerializationInfo info, StreamingContext context)
		{
			string[] entityErrors = this.EntityErrors.ToArray();

			Dictionary<string, string[]> propertyErrors = new Dictionary<string, string[]>(this.PropertyErrors.Comparer);
			this.PropertyErrors.ForEach(x => propertyErrors.Add(x.Key, x.Value.ToArray()));

			info.AddValue(nameof(RepositorySetErrors.EntityErrors), entityErrors);
			info.AddValue(nameof(RepositorySetErrors.PropertyErrors), propertyErrors);
		}

		#endregion




		#region Interface: ISerializable

		/// <inheritdoc />
		void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException(nameof(info));
			}

			this.GetObjectData(info, context);
		}

		#endregion
	}
}
