﻿using System;




namespace RI.Framework.Windows.Interop
{
	[Serializable]
	internal enum WindowsError
	{
		ErrorAccessDenied = 5,

		ErrorAlreadyAssigned = 85,

		ErrorBadDevType = 66,

		ErrorBadDevice = 1200,

		ErrorBadNetName = 67,

		ErrorBadProfile = 1206,

		ErrorBadProvider = 1204,

		ErrorBusy = 170,

		ErrorCancelled = 1223,

		ErrorCannotOpenProfile = 1205,

		ErrorDeviceAlreadyRemembered = 1202,

		ErrorExtendedError = 1208,

		ErrorInvalidPassword = 86,

		ErrorNoNetOrBadPath = 1203,

		ErrorNoNetwork = 1222,

		ErrorSuccess = 0,

		ErrorNotAllAssigned = 1300,

		ErrorInsufficientBuffer = 122,

		ErrorNoneMapped = 1332,
	}
}
