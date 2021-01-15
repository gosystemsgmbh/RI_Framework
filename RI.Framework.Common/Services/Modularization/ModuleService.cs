using System;
using System.Collections.Generic;

using RI.Framework.Collections;
using RI.Framework.Collections.DirectLinq;
using RI.Framework.Composition;
using RI.Framework.Composition.Model;
using RI.Framework.Utilities.Logging;




namespace RI.Framework.Services.Modularization
{
    /// <summary>
    ///     Implements a default modularization service which is suitable for most scenarios.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This modularization service manages <see cref="IModule" />s from two sources.
    ///         One are the explicitly specified modules added through <see cref="AddModule" />.
    ///         The second is a <see cref="CompositionContainer" /> if this <see cref="ModuleService" /> is added as an export (the modules are then imported through composition).
    ///         <see cref="Modules" /> gives the sequence containing all modules from all sources.
    ///     </para>
    ///     <para>
    ///         See <see cref="IModuleService" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    [Export]
    public sealed class ModuleService : LogSource, IModuleService, IImporting
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="ModuleService" />.
        /// </summary>
        public ModuleService ()
        {
            this.IsInitialized = false;

            this.ModulesManual = new List<IModule>();
            this.ModulesUpdated = new List<IModule>();
        }

        #endregion




        #region Instance Properties/Indexer

        [Import(typeof(IModule), Recomposable = true)]
        private Import ModulesImported { get; set; }

        private List<IModule> ModulesManual { get; set; }

        private List<IModule> ModulesUpdated { get; set; }

        #endregion




        #region Instance Methods

        private void UpdateModules ()
        {
            this.Log(LogLevel.Debug, "Updating modules");

            HashSet<IModule> currentModules = new HashSet<IModule>(this.Modules);
            HashSet<IModule> lastModules = new HashSet<IModule>(this.ModulesUpdated);

            HashSet<IModule> newModules = currentModules.Except(lastModules);
            HashSet<IModule> oldModules = lastModules.Except(currentModules);

            this.ModulesUpdated.Clear();
            this.ModulesUpdated.AddRange(currentModules);

            foreach (IModule module in newModules)
            {
                this.Log(LogLevel.Debug, "Module added: {0}", module.GetType().Name);
            }

            foreach (IModule module in oldModules)
            {
                if (module.IsInitialized)
                {
                    module.Unload();
                }

                this.Log(LogLevel.Debug, "Module removed: {0}", module.GetType().Name);
            }

            foreach (IModule module in currentModules)
            {
                if (this.IsInitialized)
                {
                    if (!module.IsInitialized)
                    {
                        module.Initialize();
                    }
                }
                else
                {
                    if (module.IsInitialized)
                    {
                        module.Unload();
                    }
                }
            }
        }

        #endregion




        #region Interface: IImporting

        /// <inheritdoc />
        void IImporting.ImportsResolved (CompositionFlags composition, bool updated)
        {
            if (updated)
            {
                this.UpdateModules();
            }
        }

        /// <inheritdoc />
        void IImporting.ImportsResolving (CompositionFlags composition)
        {
        }

        #endregion




        #region Interface: IModuleService

        /// <inheritdoc />
        public bool IsInitialized { get; private set; }

        /// <inheritdoc />
        public IEnumerable<IModule> Modules
        {
            get
            {
                foreach (IModule module in this.ModulesManual)
                {
                    yield return module;
                }

                foreach (IModule module in this.ModulesImported.Values<IModule>())
                {
                    yield return module;
                }
            }
        }

        /// <inheritdoc />
        public void AddModule (IModule module)
        {
            if (module == null)
            {
                throw new ArgumentNullException(nameof(module));
            }

            if (this.ModulesManual.Contains(module))
            {
                return;
            }

            this.ModulesManual.Add(module);

            this.UpdateModules();
        }

        /// <inheritdoc />
        public void Initialize ()
        {
            this.IsInitialized = true;

            this.UpdateModules();
        }

        /// <inheritdoc />
        public void RemoveModule (IModule module)
        {
            if (module == null)
            {
                throw new ArgumentNullException(nameof(module));
            }

            if (!this.ModulesManual.Contains(module))
            {
                return;
            }

            this.ModulesManual.RemoveAll(module);

            this.UpdateModules();
        }

        /// <inheritdoc />
        public void Unload ()
        {
            this.IsInitialized = false;

            this.UpdateModules();
        }

        #endregion
    }
}
