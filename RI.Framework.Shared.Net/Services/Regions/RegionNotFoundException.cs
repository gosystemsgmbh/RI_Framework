using System;
using System.Runtime.Serialization;

using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Services.Regions
{
	/// <summary>
	///     The <see cref="RegionNotFoundException" /> is thrown when a specified region cannot be found.
	/// </summary>
	[Serializable]
	public class RegionNotFoundException : Exception
	{
		#region Constants

		private const string ExceptionMessage = "The region does not exist: {0}.";

		#endregion




		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="RegionNotFoundException" />.
		/// </summary>
		/// <param name="region"> The name of the region which could not be found. </param>
		public RegionNotFoundException (string region)
			: base(string.Format(RegionNotFoundException.ExceptionMessage, region))
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="EmptyStringArgumentException" />.
		/// </summary>
		/// <param name="info"> The serialization data. </param>
		/// <param name="context"> The type of the source of the serialization data. </param>
		protected RegionNotFoundException (SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		#endregion
	}
}
