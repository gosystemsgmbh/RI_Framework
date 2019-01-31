using RI.Framework.Utilities.Logging;




namespace RI.Framework.Services.Modularization
{
    /// <summary>
    ///     Implements a base class which can be used for module implementation.
    /// </summary>
    /// <para>
    ///     See <see cref="IModule" /> for more details.
    /// </para>
    /// <threadsafety static="true" instance="true" />
    public abstract class ModuleBase : LogSource, IModule
    {
        #region Virtuals

        /// <inheritdoc cref="IModule.Initialize" />
        protected virtual void Initialize ()
        {
        }

        /// <inheritdoc cref="IModule.Unload" />
        protected virtual void Unload ()
        {
        }

        #endregion




        #region Interface: IModule

        /// <inheritdoc />
        public bool IsInitialized { get; private set; }

        /// <inheritdoc />
        void IModule.Initialize ()
        {
            this.Log(LogLevel.Debug, "Initializing module");

            this.Initialize();

            this.IsInitialized = true;
        }

        /// <inheritdoc />
        void IModule.Unload ()
        {
            this.Log(LogLevel.Debug, "Unloading module");

            this.Unload();

            this.IsInitialized = false;
        }

        #endregion
    }
}
