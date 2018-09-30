using System.Windows;

using RI.Framework.Composition.Model;
using RI.Framework.Services.Logging;
using RI.Framework.Utilities.Logging;




namespace RI.Framework.Bootstrapping
{
	/// <summary>
	///     Implements a default WPF application object.
	/// </summary>
	/// <remarks>
	///     <para>
	///         This default WPF application object inherits from <see cref="Application" /> and adds some common desktop application functionality such as handling of power events, logoff, sleep, etc.
	///     </para>
	/// </remarks>
	/// <threadsafety static="false" instance="false" />
	/// TODO: Add promised features (power events, logoff, sleep, etc.)
	[Export]
	[Export(typeof(Application))]
	public class WpfApplication : Application, ILogSource
	{
		#region Interface: ILogSource

		/// <inheritdoc />
		public LogLevel LogFilter { get; set; } = LogLevel.Debug;

		/// <inheritdoc />
		public ILogger Logger { get; set; } = LogLocator.Logger;

		/// <inheritdoc />
		public bool LoggingEnabled { get; set; } = true;

		#endregion
	}
}
