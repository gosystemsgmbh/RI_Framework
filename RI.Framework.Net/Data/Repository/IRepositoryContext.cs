using System;

using RI.Framework.Data.Repository.Entities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Data.Repository
{
	/// <summary>
	///     Defines the interface for a repository context.
	/// </summary>
	/// <remarks>
	///     <para>
	///         A repository context encapsulates high-level database functionality by representing a repository and unit-of-work pattern.
	///     </para>
	///     <para>
	///         A repository context encapsulates database access and logic and provides higher-level functionality by exposing context-specific data operations and by exposing the actual data using entities.
	///     </para>
	///     <para>
	///         Repository contexts are intended to be short-living instances which are created when necessary and disposed after the data-specific tasks are completed (&quot;unit of work&quot;).
	///     </para>
	/// </remarks>
	public interface IRepositoryContext : IDisposable
	{
		/// <summary>
		///     Commits all pending changes since the last <see cref="SaveChanges" /> and disposes the repository context.
		/// </summary>
		/// <exception cref="ObjectDisposedException"> The repository context is disposed. </exception>
		void Commit ();

		/// <summary>
		///     Gets the non-generic repository set for a certain type of entities.
		/// </summary>
		/// <param name="type"> The type of entities. </param>
		/// <returns>
		///     The repository set for the entities of type <paramref name="type" />.
		/// </returns>
		/// <exception cref="ObjectDisposedException"> The repository context is disposed. </exception>
		/// <exception cref="ArgumentNullException"> <paramref name="type" /> is null. </exception>
		/// <exception cref="InvalidTypeArgumentException"> The type <paramref name="type" /> does not represent a valid entity type for which a repository set can be retrieved. </exception>
		IRepositorySet GetSet (Type type);

		/// <summary>
		///     Gets the generic repository set for a certain type of entities.
		/// </summary>
		/// <typeparam name="T"> The type of entities. </typeparam>
		/// <returns>
		///     The repository set for the entities of type <typeparamref name="T" />.
		/// </returns>
		/// <exception cref="ObjectDisposedException"> The repository context is disposed. </exception>
		/// <exception cref="InvalidTypeArgumentException"> The type <typeparamref name="T" /> does not represent a valid entity type for which a repository set can be retrieved. </exception>
		IRepositorySet<T> GetSet <T> () where T : class;

		/// <summary>
		///     Determines whether the repository has any pending changes, meaning that one or more of its entities were modified.
		/// </summary>
		/// <returns>
		///     true if the repository has pending changes, false otherwise.
		/// </returns>
		/// <exception cref="ObjectDisposedException"> The repository context is disposed. </exception>
		bool HasChanges ();

		/// <summary>
		///     Determines whether the repository has any validation errors, meaning that one or more of its entities have invalid data.
		/// </summary>
		/// <returns>
		///     true if the repository has validation errors, false otherwise.
		/// </returns>
		/// <exception cref="ObjectDisposedException"> The repository context is disposed. </exception>
		bool HasErrors ();

		/// <summary>
		///     Rolls back all pending changes since the last <see cref="SaveChanges" /> and disposes the repository context.
		/// </summary>
		/// <exception cref="ObjectDisposedException"> The repository context is disposed. </exception>
		void Rollback ();

		/// <summary>
		///     Saves all pending changes of all entities without disposing the repository context.
		/// </summary>
		/// <remarks>
		///     <para>
		///         There will be no more pending changes afterwards.
		///     </para>
		/// </remarks>
		/// <exception cref="ObjectDisposedException"> The repository context is disposed. </exception>
		void SaveChanges ();

		/// <summary>
		/// Raised to resolve the currently used change tracking context for <see cref="IEntityChangeTracking"/> when <see cref="Commit"/> or <see cref="SaveChanges"/> is executed.
		/// </summary>
		/// <remarks>
		/// <para>
		/// <see cref="ChangeTrackingContextResolve"/> is always called, regardless whether the processed entities support <see cref="IEntityChangeTracking"/> or not.
		/// </para>
		/// </remarks>
		event EventHandler<ChangeTrackingContextResolveEventArgs> ChangeTrackingContextResolve;
	}
}
