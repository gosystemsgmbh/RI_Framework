using System;




namespace RI.Framework.Data
{
	[Serializable]
	public enum DatabaseState
	{
		Uninitialized = 0,

		Ready = 1,

		New = 2,

		Old = 3,

		TooNew = 4,

		TooOld = 5,

		DamagedOrInvalid = 6,
	}
}