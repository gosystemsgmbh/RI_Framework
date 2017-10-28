using System;

using RI.Framework.IO.Paths;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Services
{
	/// <summary>
	///     Describes the desired shutdown information returned by a bootstrapper.
	/// </summary>
	public class ShutdownInfo : ICloneable<ShutdownInfo>, ICloneable
	{
		#region Instance Properties/Indexer

		/// <summary>
		///     Gets or sets the process exit code.
		/// </summary>
		/// <value>
		///     The process exit code.
		/// </value>
		public int ExitCode { get; set; } = 0;

		/// <summary>
		///     Gets or sets the shutdown mode.
		/// </summary>
		/// <value>
		///     The shutdown mode.
		/// </value>
		public ShutdownMode Mode { get; set; } = ShutdownMode.ExitApplication;

		/// <summary>
		///     Gets or sets the arguments for the script file.
		/// </summary>
		/// <value>
		///     The arguments for the script file.
		/// </value>
		/// <remarks>
		///     <para>
		///         <see cref="ScriptArguments" /> is ignored if <see cref="ScriptFile" /> is null or <see cref="Mode" /> is not <see cref="ShutdownMode.ExitApplicationAndRunScript" />.
		///     </para>
		/// </remarks>
		public string ScriptArguments { get; set; } = null;

		/// <summary>
		///     Gets or sets the script file to execute after shutdown.
		/// </summary>
		/// <value>
		///     The script file to execute after shutdown.
		/// </value>
		/// <remarks>
		///     <para>
		///         <see cref="ScriptFile" /> is ignored if <see cref="Mode" /> is not <see cref="ShutdownMode.ExitApplicationAndRunScript" />.
		///     </para>
		/// </remarks>
		public FilePath ScriptFile { get; set; } = null;

		#endregion




		#region Interface: ICloneable<ShutdownInfo>

		/// <inheritdoc />
		public virtual ShutdownInfo Clone ()
		{
			ShutdownInfo clone = new ShutdownInfo();
			clone.Mode = this.Mode;
			clone.ExitCode = this.ExitCode;
			clone.ScriptFile = this.ScriptFile?.Clone();
			clone.ScriptArguments = this.ScriptArguments;
			return clone;
		}

		/// <inheritdoc />
		object ICloneable.Clone ()
		{
			return this.Clone();
		}

		#endregion
	}
}
