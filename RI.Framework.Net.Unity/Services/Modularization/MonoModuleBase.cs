using System;

using RI.Framework.Composition.Model;
using RI.Framework.Services.Logging;
using RI.Framework.Utilities.Logging;

using UnityEngine;

using Object = UnityEngine.Object;




namespace RI.Framework.Services.Modularization
{
    /// <summary>
    ///     Implements a base class which can be used for <c> MonoBehaviour </c> based module implementation.
    /// </summary>
    /// <remarks>
    ///     <note type="note">
    ///         Instances of <see cref="MonoModuleBase" />s are not created using their constructor (as this would be the wrong way how to instantiate a <c> MonoBehaviour </c>). Instead, <see cref="CreateInstance" /> is used.
    ///     </note>
    ///     <para>
    ///         See <see cref="IModule" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    public abstract class MonoModuleBase : MonoBehaviour, IModule, ILogSource
    {
        #region Static Methods

        /// <summary>
        ///     Creates an instance of the specified <see cref="MonoModuleBase" /> type.
        /// </summary>
        /// <param name="type"> The type of which an instance is to be created. </param>
        /// <returns> The created instance. </returns>
        /// <remarks>
        ///     <para>
        ///         To instantiate a <see cref="MonoModuleBase" />, a new <c> GameObject </c> is created to which the <see cref="MonoModuleBase" /> is added as a component using <c> AddComponent </c>.
        ///         The created <c> GameObject </c> has also called <c> Object.DontDestroyOnLoad </c> on it.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="type" /> is null. </exception>
        [ExportCreator]
        public static MonoModuleBase CreateInstance (Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            GameObject gameObject = new GameObject();
            gameObject.name = type.Name;
            MonoModuleBase instance = gameObject.AddComponent(type) as MonoModuleBase;
            Object.DontDestroyOnLoad(gameObject);
            return instance;
        }

        #endregion




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




        #region Interface: ILogSource

        /// <inheritdoc />
        public LogLevel LogFilter { get; set; } = LogLevel.Debug;

        /// <inheritdoc />
        public Utilities.Logging.ILogger Logger { get; set; } = LogLocator.Logger;


        /// <inheritdoc />
        public bool LoggingEnabled { get; set; } = true;

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
