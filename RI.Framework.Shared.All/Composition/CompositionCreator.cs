using System;

using RI.Framework.Composition.Model;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.Logging;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Composition
{
	/// <summary>
	///     Implements the base class for composition creators.
	/// </summary>
	/// <remarks>
	///     <para>
	///         Composition creators can be used to create instances of types which are xported to a <see cref="CompositionContainer" />.
	///     </para>
	///     <para>
	///         Composition creators are one of three ways how instances of exported types can be created, besides export constructors, including <see cref="ExportConstructorAttribute" />, and export creator methods using <see cref="ExportCreatorAttribute" />.
	///     </para>
	///     <para>
	///         Composition creators must be manually added to a <see cref="CompositionContainer" /> and are not resolved through composition themselves.
	///     </para>
	///     <para>
	///         A composition creator can be added to multiple <see cref="CompositionContainer" />s.
	///     </para>
	///     <para>
	///         See <see cref="CompositionContainer" /> for more details about composition.
	///     </para>
	///     <note type="important">
	///         Some virtual methods are called from within locks to <see cref="SyncRoot" /> and/or <see cref="CompositionContainer.SyncRoot" />.
	///         Be careful in inheriting classes when calling outside code from those methods (e.g. through events, callbacks, or other virtual methods) to not produce deadlocks!
	///     </note>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public abstract class CompositionCreator : LogSource, IDisposable, ISynchronizable
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="CompositionCreator" />.
		/// </summary>
		protected CompositionCreator ()
		{
			this.SyncRoot = new object();
		}

		/// <summary>
		///     Garbage collects this instance of <see cref="CompositionCreator" />.
		/// </summary>
		~CompositionCreator ()
		{
			this.Dispose(false);
		}

		#endregion




		#region Abstracts

		/// <summary>
		///     Determines whether this composition creator can create an instance of a specified type.
		/// </summary>
		/// <param name="container"> The composition container which is currently using this composition creator to create an instance of <paramref name="type" />. </param>
		/// <param name="type"> The type to create an instance of. </param>
		/// <param name="compatibleType"> The type to which the created instance is eventually assigned. </param>
		/// <param name="exportName"> The name under which the type is exported. </param>
		/// <returns>
		///     true if this composition creator can create an instance of <paramref name="type" />, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="type" />, <paramref name="compatibleType" />, or <paramref name="exportName" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="exportName" /> is an empty string. </exception>
		protected internal abstract bool CanCreateInstance (CompositionContainer container, Type type, Type compatibleType, string exportName);

		/// <summary>
		///     Attempts to create an instance of a specified type
		/// </summary>
		/// <param name="container"> The composition container which is currently using this composition creator to create an instance of <paramref name="type" />. </param>
		/// <param name="type"> The type to create an instance of. </param>
		/// <param name="compatibleType"> The type to which the created instance is eventually assigned. </param>
		/// <param name="exportName"> The name under which the type is exported. </param>
		/// <returns>
		///     The created instance of <paramref name="type" /> or null if this composition creator cannot create an instance of <paramref name="type" />.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="type" />, <paramref name="compatibleType" />, or <paramref name="exportName" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="exportName" /> is an empty string. </exception>
		protected internal abstract object CreateInstance (CompositionContainer container, Type type, Type compatibleType, string exportName);

		#endregion




		#region Virtuals

		/// <summary>
		///     Disposes this catalog and frees all resources.
		/// </summary>
		/// <param name="disposing"> true if called from <see cref="IDisposable.Dispose" />, false if called from the destructor. </param>
		protected virtual void Dispose (bool disposing)
		{
		}

		#endregion




		#region Interface: IDisposable

		/// <inheritdoc />
		void IDisposable.Dispose ()
		{
			this.Dispose(true);
		}

		#endregion




		#region Interface: ISynchronizable

		/// <inheritdoc />
		bool ISynchronizable.IsSynchronized => true;

		/// <inheritdoc />
		public object SyncRoot { get; }

		#endregion
	}
}
