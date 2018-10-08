using System;
using System.Globalization;
using System.Threading.Tasks;




namespace RI.Framework.Threading.Async
{
	/// <summary>
	///     Implements an awaiter which flows the current <see cref="CultureInfo" /> around the execution of a <see cref="Task" />.
	/// </summary>
	/// <threadsafety static="true" instance="true" />
	public sealed class CultureFlower : CustomFlower
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="CultureFlower" />.
		/// </summary>
		/// <param name="task"> The <see cref="Task" /> around which flows the current <see cref="CultureInfo" />. </param>
		/// <exception cref="ArgumentNullException"> <paramref name="task" /> is null. </exception>
		public CultureFlower (Task task)
			: base(task)
		{
			this.FormattingCulture = null;
			this.UiCulture = null;
		}

		#endregion




		#region Instance Properties/Indexer

		private CultureInfo FormattingCulture { get; set; }
		private CultureInfo UiCulture { get; set; }

		#endregion




		#region Overrides

		/// <inheritdoc />
		protected override void Capture ()
		{
			this.FormattingCulture = CultureInfo.CurrentCulture;
			this.UiCulture = CultureInfo.CurrentUICulture;
		}

		/// <inheritdoc />
		protected override void Restore ()
		{
			CultureInfo.CurrentCulture = this.FormattingCulture;
			CultureInfo.CurrentUICulture = this.UiCulture;
		}

		#endregion
	}
}
