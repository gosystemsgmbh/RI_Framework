using System.Windows;

using RI.Framework.Composition.Model;
using RI.Framework.Services.Logging;




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
	public class WpfApplication : Application
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




		#region Instance Methods

		/// <summary>
		///     Logs a message.
		/// </summary>
		/// <param name="severity"> The severity of the message. </param>
		/// <param name="format"> The message. </param>
		/// <param name="args"> The arguments which will be expanded into the message (comparable to <see cref="string.Format(string, object[])" />). </param>
		/// <remarks>
		///     <para>
		///         <see cref="ILogService" /> is used, obtained through <see cref="ServiceLocator" />.
		///         If no <see cref="ILogService" /> is available, no logging is performed.
		///     </para>
		/// </remarks>
		protected void Log (LogLevel severity, string format, params object[] args)
		{
			LogLocator.Log(severity, this.GetType().Name, format, args);
		}

		#endregion
	}
}
