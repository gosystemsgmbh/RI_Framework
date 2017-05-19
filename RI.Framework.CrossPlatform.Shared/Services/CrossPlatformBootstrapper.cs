using System;

using RI.Framework.Utilities.CrossPlatform;

namespace RI.Framework.Services
{
	/// <summary>
	/// Implements a cross-platform application bootstrapper.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="Bootstrapper" /> for more details.
	///     </para>
	/// </remarks>
	public abstract class CrossPlatformBootstrapper : Bootstrapper
	{
		/// <summary>
		///     Called when a default application object needs to be created.
		/// </summary>
		/// <returns>
		///     The default application object.
		///     Can be null if the use of an application object is not applicable.
		/// </returns>
		/// <remarks>
		///     <para>
		///         <see cref="CreateDefaultApplication" /> is called if <see cref="Bootstrapper.CreateApplication" /> returns null to use a default application object.
		///     </para>
		///     <note type="implement">
		///         The default implementation returns null.
		///     </note>
		/// </remarks>
		protected override object CreateDefaultApplication()
		{
			return null;
		}

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
		protected override Guid DetermineDomainId()
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
		protected override Guid DetermineMachineId()
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
		protected override Guid DetermineUserId()
		{
			return UniqueIdentification.GetUserId();
		}
	}
}
