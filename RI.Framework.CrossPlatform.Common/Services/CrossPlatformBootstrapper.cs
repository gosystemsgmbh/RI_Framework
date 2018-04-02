using System;

using RI.Framework.Bootstrapping;
using RI.Framework.CrossPlatform.Cryptography;
using RI.Framework.CrossPlatform.Users;




namespace RI.Framework.Services
{
	/// <summary>
	///     Implements a cross-platform application bootstrapper.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="Bootstrapper" /> for more details.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public abstract class CrossPlatformBootstrapper : Bootstrapper
	{
		#region Overrides

		/// <inheritdoc />
		protected override Guid DetermineDomainId ()
		{
			return UniqueIdentification.GetDomainId();
		}

		/// <inheritdoc />
		protected override Guid DetermineMachineId ()
		{
			return UniqueIdentification.GetMachineId();
		}

		/// <inheritdoc />
		protected override bool DetermineStartupUserElevated ()
		{
			return CrossPlatformUser.IsCurrentElevated();
		}

		/// <inheritdoc />
		protected override Guid DetermineUserId ()
		{
			return UniqueIdentification.GetUserId();
		}

		#endregion
	}
}
