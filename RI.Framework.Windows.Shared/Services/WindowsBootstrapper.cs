using System;

using RI.Framework.Utilities.Windows.Cryptography;




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

		/// <summary>
		///     Called to determine the GUID of the domain this machine belongs to.
		/// </summary>
		/// <returns>
		///     The GUID of the the domain this machine belongs to.
		/// </returns>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation uses <see cref="UniqueIdentification" />.<see cref="UniqueIdentification.GetDomainId" />.
		///     </note>
		/// </remarks>
		protected override Guid DetermineDomainId ()
		{
			return UniqueIdentification.GetDomainId();
		}

		/// <summary>
		///     Called to determine the GUID of the local machine.
		/// </summary>
		/// <returns>
		///     The GUID of the local machine.
		/// </returns>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation uses <see cref="UniqueIdentification" />.<see cref="UniqueIdentification.GetMachineId" />.
		///     </note>
		/// </remarks>
		protected override Guid DetermineMachineId ()
		{
			return UniqueIdentification.GetMachineId();
		}

		/// <summary>
		///     Called to determine the GUID of the current user.
		/// </summary>
		/// <returns>
		///     The GUID of the current user.
		/// </returns>
		/// <remarks>
		///     <note type="implement">
		///         The default implementation uses <see cref="UniqueIdentification" />.<see cref="UniqueIdentification.GetUserId" />.
		///     </note>
		/// </remarks>
		protected override Guid DetermineUserId ()
		{
			return UniqueIdentification.GetUserId();
		}

		#endregion
	}
}
