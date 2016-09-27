using System.Windows;
using System.Windows.Threading;

using RI.Framework.Composition.Model;




namespace RI.Framework.Services.Modularization
{
	/// <summary>
	///     Implements a base class which can be used for module implementation in WPF applications.
	/// </summary>
	[Export]
	public abstract class WpfModule : Module
	{
		#region Instance Properties/Indexer

		/// <summary>
		///     Gets the used WPF application object.
		/// </summary>
		/// <value>
		///     The used WPF application object.
		/// </value>
		protected Application Application => this.Bootstrapper.Application;

		/// <summary>
		///     Gets the used <see cref="WpfBootstrapper" />.
		/// </summary>
		/// <value>
		///     The used <see cref="WpfBootstrapper" />.
		/// </value>
		[ImportProperty]
		protected WpfBootstrapper Bootstrapper { get; private set; }

		/// <summary>
		///     Gets the dispatcher of the WPF application object.
		/// </summary>
		/// <value>
		///     The dispatcher of the WPF application object.
		/// </value>
		protected Dispatcher Dispatcher => this.Application.Dispatcher;

		#endregion
	}
}
