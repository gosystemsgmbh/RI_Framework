using System;

using RI.Framework.Windows.Cryptography;
using RI.Framework.Windows.Users;




namespace RI.Framework.Services
{
	/// <summary>
	///     Implements a Windows application bootstrapper.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="Bootstrapper{TApplication}" /> for more details.
	///     </para>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public abstract class WindowsBootstrapper <TApplication> : Bootstrapper<TApplication>
		where TApplication : class
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
			return WindowsUser.IsCurrentAdministrator();
		}

		/// <inheritdoc />
		protected override Guid DetermineUserId ()
		{
			return UniqueIdentification.GetUserId();
		}

		#endregion
	}
}
