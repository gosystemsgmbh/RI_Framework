using System.Windows;

using RI.Framework.Composition.Model;
using RI.Framework.Services.Logging;
using RI.Framework.Utilities.Logging;




namespace RI.Framework.Services
{
	/// <summary>
	///     Implements a default WPF application object.
	/// </summary>
	/// <remarks>
	///     <para>
	///         This default WPF application object inherits from <see cref="Application" /> and adds some common desktop application functionality such as handling of power events, logoff, sleep, etc.
	///     </para>
	/// </remarks>
	[Export]
	public class WpfApplication : Application, ILogSource
	{
		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the used bootstrapper object.
		/// </summary>
		/// <value>
		///     The used bootstrapper object.
		/// </value>
		public Bootstrapper Bootstrapper { get; internal set; }

		#endregion




		#region Interface: ILogSource

		/// <inheritdoc />
		public ILogger Logger { get; set; } = LogLocator.Logger;


		/// <inheritdoc />
		public bool LoggingEnabled { get; set; } = true;

		#endregion
	}
}
