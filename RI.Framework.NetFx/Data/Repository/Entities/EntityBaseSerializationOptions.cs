using System;




namespace RI.Framework.Data.Repository.Entities
{
	/// <summary>
	/// Defines the serialization options for <see cref="EntityBase"/>.
	/// </summary>
	[Serializable]
	[Flags]
	public enum EntityBaseSerializationOptions
	{
		/// <summary>
		/// Nothing gets serialized.
		/// </summary>
		None = 0x00,

		/// <summary>
		/// Errors are serialized.
		/// </summary>
		Errors = 0x01,

		/// <summary>
		/// Creation tracking is serialized.
		/// </summary>
		CreateTracking = 0x02,

		/// <summary>
		/// Modification tracking is serialized.
		/// </summary>
		ModifyTracking = 0x04,

		/// <summary>
		/// All public properties are serialized.
		/// </summary>
		PublicProperties = 0x08,

		/// <summary>
		/// The used serialization options are also serialized.
		/// </summary>
		SerializationOptions = 0x10,

		/// <summary>
		/// Everyting is serialized (comination of <see cref="Errors"/>, <see cref="CreateTracking"/>, <see cref="ModifyTracking"/>, <see cref="PublicProperties"/>, <see cref="SerializationOptions"/>).
		/// </summary>
		All = EntityBaseSerializationOptions.Errors | EntityBaseSerializationOptions.CreateTracking | EntityBaseSerializationOptions.ModifyTracking | EntityBaseSerializationOptions.PublicProperties | EntityBaseSerializationOptions.SerializationOptions,
	}
}
