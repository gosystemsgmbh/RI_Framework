﻿using RI.Framework.Data.EF.Configuration;




namespace RI.Framework.Data.Repository.Entities
{
	/// <summary>
	/// Implements a base class for entity configuration which defines default behaviour for <see cref="EntityBase"/> based entities.
	/// </summary>
	/// <typeparam name="T"> The type of entity this configuration configures. </typeparam>
	/// <remarks>
	///     <para>
	///         See <see cref="EntityConfiguration{T}" /> for more details.
	///     </para>
	/// <note note="note">
	/// By default, the following properties are ignored / not mapped to columns:
	/// <see cref="EntityBase.SerializationOptions"/>, <see cref="EntityBase.Errors"/>, <see cref="EntityBase.ErrorStringWithSpaces"/>, <see cref="EntityBase.ErrorStringWithNewLines"/>.
	/// </note>
	/// </remarks>
	public abstract class EntityConfigurationBase<T> : EntityConfiguration<T>
		where T : EntityBase
	{
		/// <summary>
		/// Creates a new instance of <see cref="EntityConfigurationBase{T}"/>.
		/// </summary>
		protected EntityConfigurationBase ()
		{
			this.Ignore(x => x.SerializationOptions);

			this.Ignore(x => x.Errors);
			this.Ignore(x => x.ErrorStringWithSpaces);
			this.Ignore(x => x.ErrorStringWithNewLines);
		}

		/// <summary>
		/// Ignores / does not map creation tracking (<see cref="EntityBase.CreateTimestamp"/>, <see cref="EntityBase.CreateContext"/>).
		/// </summary>
		public void IgnoreCreateTracking ()
		{
			this.Ignore(x => x.CreateTimestamp);
			this.Ignore(x => x.CreateContext);
		}

		/// <summary>
		/// Ignores / does not map modification tracking (<see cref="EntityBase.ModifyTimestamp"/>, <see cref="EntityBase.ModifyContext"/>).
		/// </summary>
		public void IgnoreModifyTracking ()
		{
			this.Ignore(x => x.ModifyTimestamp);
			this.Ignore(x => x.ModifyContext);
		}
	}
}
