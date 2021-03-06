﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;




namespace RI.Framework.Windows.Wpf
{
	/// <summary>
	///     Provides temporary global mouse cursor overrides.
	/// </summary>
	/// <remarks>
	///     <see cref="TemporaryCursor" /> can be used to temporarily show a specific mosue cursor for an application while a certain section of code is running.
	///     To achieve this, <see cref="TemporaryCursor" /> implements <see cref="IDisposable" /> which allows the using directive and therefore ensures that the mouse cursor will be reset to its original state.
	/// </remarks>
	/// <example>
	///     <para>
	///         The following example shows how <see cref="TemporaryCursor" /> can be used:
	///     </para>
	///     <code language="cs">
	///  <![CDATA[
	///  using (TemporaryCursor.Wait())
	///  {
	/// 		//The hourglass cursor is shown while this section of code is running.
	///  }
	///  ]]>
	///  </code>
	/// </example>
	public sealed class TemporaryCursor : IDisposable
	{
		#region Static Methods

		/// <summary>
		///     Shows the arrow cursor with a small hourglass.
		/// </summary>
		/// <returns>
		///     The <see cref="TemporaryCursor" /> object which is to be disposed when the original cursor needs to be restored.
		/// </returns>
		public static TemporaryCursor AppStarting ()
		{
			return new TemporaryCursor(Cursors.AppStarting);
		}

		/// <summary>
		///     Shows the hourglass cursor.
		/// </summary>
		/// <returns>
		///     The <see cref="TemporaryCursor" /> object which is to be disposed when the original cursor needs to be restored.
		/// </returns>
		public static TemporaryCursor Wait ()
		{
			return new TemporaryCursor(Cursors.Wait);
		}

		#endregion




		#region Instance Constructor/Destructor

		private TemporaryCursor (Cursor cursor)
		{
			this.PreviousCursor = Mouse.OverrideCursor;
			Mouse.OverrideCursor = cursor;
		}

		/// <summary>
		///     Garbage collects this instance of <see cref="TemporaryCursor" />.
		/// </summary>
		~TemporaryCursor ()
		{
			this.Dispose(false);
		}

		#endregion




		#region Instance Properties/Indexer

		private Cursor PreviousCursor { get; set; }

		#endregion




		#region Instance Methods

		/// <summary>
		///     Restores the original cursor before this instance was created.
		/// </summary>
		public void Restore ()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		[SuppressMessage("ReSharper", "UnusedParameter.Local")]
		private void Dispose (bool disposing)
		{
			Mouse.OverrideCursor = this.PreviousCursor;
			this.PreviousCursor = null;
		}

		#endregion




		#region Interface: IDisposable

		/// <summary>
		///     Restores the original cursor which was saved when this <see cref="TemporaryCursor" /> was created.
		/// </summary>
		void IDisposable.Dispose ()
		{
			this.Restore();
		}

		#endregion
	}
}
