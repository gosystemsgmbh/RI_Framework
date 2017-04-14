namespace RI.Framework.Mathematic
{
	/// <summary>
	///     Mathematical functions.
	/// </summary>
	public static class MathUtils
	{
		#region Static Methods

		/// <summary>
		///     Converts a radians value to degree.
		/// </summary>
		/// <param name="radians"> The value. </param>
		/// <returns>
		///     The value in degree.
		/// </returns>
		public static double ToDeg (this double radians) => radians * MathConstD.RadToDeg;

		/// <summary>
		///     Converts a radians value to degree.
		/// </summary>
		/// <param name="radians"> The value. </param>
		/// <returns>
		///     The value in degree.
		/// </returns>
		public static float ToDeg (this float radians) => radians * MathConstF.RadToDeg;

		/// <summary>
		///     Converts a degree value to radians.
		/// </summary>
		/// <param name="degree"> The value. </param>
		/// <returns>
		///     The value in radians.
		/// </returns>
		public static double ToRad (this double degree) => degree * MathConstD.DegToRad;

		/// <summary>
		///     Converts a degree value to radians.
		/// </summary>
		/// <param name="degree"> The value. </param>
		/// <returns>
		///     The value in radians.
		/// </returns>
		public static float ToRad (this float degree) => degree * MathConstF.DegToRad;

		#endregion
	}
}
