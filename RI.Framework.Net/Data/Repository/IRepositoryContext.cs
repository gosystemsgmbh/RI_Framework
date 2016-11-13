using System;

using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Data.Repository
{
	/// <summary>
	///     Defines the interface for a repository context.
	/// </summary>
	/// <remarks>
	///     <para>
	///         A repository context encapsulates high-level database functionality by representing a repository / unit-of-work pattern which uses objects as entities with the associated database.
	///     </para>
	///     <para>
	///         A repository context is intended to be created, used for a certain task, and then disposed (either through a <see cref="Commit" /> or <see cref="Rollback" />).
	///         It should not be a long-living instance.
	///     </para>
	/// </remarks>
	public interface IRepositoryContext : IDisposable
	{
		/// <summary>
		///     Commits all pending changes and disposes the repository context.
		/// </summary>
		/// <exception cref="ObjectDisposedException"> The repository context is disposed. </exception>
		void Commit ();

		/// <summary>
		///     Gets the repository set for a certain type of entities.
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
		///     Rolls back all changes since the last <see cref="SaveChanges" /> call and disposes the repository context.
		/// </summary>
		/// <exception cref="ObjectDisposedException"> The repository context is disposed. </exception>
		void Rollback ();

		/// <summary>
		///     Performs a rollback of all entities which have invalid data without disposing the repository context.
		/// </summary>
		/// <remarks>
		///     <para>
		///         There will be no more validation errors afterwards.
		///     </para>
		/// </remarks>
		/// <exception cref="ObjectDisposedException"> The repository context is disposed. </exception>
		void RollbackErrors ();

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
	}
}
