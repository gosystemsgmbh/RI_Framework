using System;
using System.Collections.Generic;
using System.Configuration;

using RI.Framework.Collections.DirectLinq;
using RI.Framework.Composition.Model;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.ObjectModel;




namespace RI.Framework.Services.Settings.Storages
{
    /// <summary>
    ///     Implements a setting storage which reads from the applications default app.config file.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This setting store is read-only.
    ///     </para>
    ///     <para>
    ///         This setting store internally uses <see cref="ConfigurationManager" />.<see cref="ConfigurationManager.AppSettings" /> to read the applications default configuration, which is usually the app.config file.
    ///     </para>
    ///     <note type="important">
    ///         <see cref="AppConfigSettingStorage" /> does not support multiple values for the same setting!
    ///     </note>
    ///     <para>
    ///         See <see cref="ISettingStorage" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    [Export]
    public sealed class AppConfigSettingStorage : ISettingStorage
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="AppConfigSettingStorage" />.
        /// </summary>
        public AppConfigSettingStorage ()
        {
            this.SyncRoot = new object();
        }

        #endregion




        #region Interface: ISettingStorage

        /// <inheritdoc />
        public bool IsReadOnly => true;

        /// <inheritdoc />
        bool ISynchronizable.IsSynchronized => true;

        /// <inheritdoc />
        public object SyncRoot { get; }

        /// <inheritdoc />
        public bool WriteOnlyKnown => false;

        /// <inheritdoc />
        public IReadOnlyCollection<string> WritePrefixAffinities => null;

        /// <inheritdoc />
        public void DeleteValues (string name)
        {
            throw new NotSupportedException("Deleting a value from app.config is not supported.");
        }

        /// <inheritdoc />
        public void DeleteValues (Predicate<string> predicate)
        {
            throw new NotSupportedException("Deleting a value from app.config is not supported.");
        }

        /// <inheritdoc />
        public List<string> GetValues (string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(name));
            }

            lock (this.SyncRoot)
            {
                List<string> values = new List<string>();
                string value = ConfigurationManager.AppSettings[name];
                if (value != null)
                {
                    values.Add(value);
                }

                return values;
            }
        }

        /// <inheritdoc />
        public Dictionary<string, List<string>> GetValues (Predicate<string> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            lock (this.SyncRoot)
            {
                Dictionary<string, List<string>> values = new Dictionary<string, List<string>>(SettingService.NameComparer);
                string[] keys = ConfigurationManager.AppSettings.AllKeys;
                foreach (string key in keys)
                {
                    string value = ConfigurationManager.AppSettings[key];
                    if (predicate(key))
                    {
                        if (!values.ContainsKey(key))
                        {
                            values.Add(key, new List<string>());
                        }

                        values[key].Add(value);
                    }
                }

                return values;
            }
        }

        /// <inheritdoc />
        public bool HasValue (string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(name));
            }

            lock (this.SyncRoot)
            {
                return ConfigurationManager.AppSettings[name] != null;
            }
        }

        /// <inheritdoc />
        public bool HasValue (Predicate<string> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            lock (this.SyncRoot)
            {
                return ConfigurationManager.AppSettings.AllKeys.Any(x => predicate(x));
            }
        }

        /// <inheritdoc />
        public void Load ()
        {
        }

        /// <inheritdoc />
        public void Save ()
        {
            throw new NotSupportedException("Saving to app.config is not supported.");
        }

        /// <inheritdoc />
        public void SetValues (string name, IEnumerable<string> values)
        {
            throw new NotSupportedException("Setting a value to app.config is not supported.");
        }

        #endregion
    }
}
