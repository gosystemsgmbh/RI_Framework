using System;
using System.Collections.Generic;
using System.Linq;

using RI.Framework.Collections;
using RI.Framework.Collections.Linq;
using RI.Framework.Composition;
using RI.Framework.Composition.Model;
using RI.Framework.Services.Logging;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Services.Resources
{
	/// <summary>
	///     Implements a default resource service which is suitable for most scenarios.
	/// </summary>
	/// <remarks>
	///     <para>
	///         This resource service manages <see cref="IResourceSource" />s  and <see cref="IResourceConverter" />s from two sources.
	///         One are the explicitly specified sources and converters added through <see cref="AddSource" /> and <see cref="AddConverter" />.
	///         The second is a <see cref="CompositionContainer" /> if this <see cref="ResourceService" /> is added as an export (the sources and converters are then imported through composition).
	///         <see cref="Sources" /> gives the sequence containing all setting storages from all sources and <see cref="Converters" /> gives the sequence containing all setting converters from all sources.
	///     </para>
	///     <para>
	///         See <see cref="IResourceService" /> for more details.
	///     </para>
	/// </remarks>
	public sealed class ResourceService : IResourceService, IImporting
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="ResourceService" />.
		/// </summary>
		public ResourceService ()
		{
			this.SourcesUpdated = new List<IResourceSource>();
			this.ConvertersUpdated = new List<IResourceConverter>();

			this.SourcesManual = new List<IResourceSource>();
			this.ConvertersManual = new List<IResourceConverter>();
		}

		#endregion




		#region Instance Properties/Indexer

		[ImportProperty (typeof(IResourceConverter), Recomposable = true)]
		private Import ConvertersImported { get; set; }

		private List<IResourceConverter> ConvertersManual { get; set; }

		private List<IResourceConverter> ConvertersUpdated { get; set; }

		[ImportProperty (typeof(IResourceSource), Recomposable = true)]
		private Import SourcesImported { get; set; }

		private List<IResourceSource> SourcesManual { get; set; }

		private List<IResourceSource> SourcesUpdated { get; set; }

		#endregion




		#region Instance Methods

		private IResourceConverter GetConverterForType (Type sourceType, Type targetType)
		{
			foreach (IResourceConverter converter in this.Converters)
			{
				if (converter.CanConvert(sourceType, targetType))
				{
					return converter;
				}
			}

			return null;
		}

		private void Log (string format, params object[] args)
		{
			LogLocator.LogDebug(this.GetType().Name, format, args);
		}

		private void UpdateConverters ()
		{
			this.Log("Updating converters");

			HashSet<IResourceConverter> currentConverters = new HashSet<IResourceConverter>(this.Converters);
			HashSet<IResourceConverter> lastConverters = new HashSet<IResourceConverter>(this.ConvertersUpdated);

			HashSet<IResourceConverter> newConverters = DirectLinq.Except(currentConverters, lastConverters);
			HashSet<IResourceConverter> oldConverters = DirectLinq.Except(lastConverters, currentConverters);

			this.ConvertersUpdated.Clear();
			this.ConvertersUpdated.AddRange(currentConverters);

			foreach (IResourceConverter converter in newConverters)
			{
				this.Log("Converter added: {0}", converter.GetType().Name);
			}

			foreach (IResourceConverter converter in oldConverters)
			{
				this.Log("Converter removed: {0}", converter.GetType().Name);
			}

			this.UpdateAvailable();
		}

		private void UpdateSources ()
		{
			this.Log("Updating sources");

			HashSet<IResourceSource> currentSources = new HashSet<IResourceSource>(this.Sources);
			HashSet<IResourceSource> lastSources = new HashSet<IResourceSource>(this.SourcesUpdated);

			HashSet<IResourceSource> newSources = DirectLinq.Except(currentSources, lastSources);
			HashSet<IResourceSource> oldSources = DirectLinq.Except(lastSources, currentSources);

			this.SourcesUpdated.Clear();
			this.SourcesUpdated.AddRange(currentSources);

			foreach (IResourceSource source in newSources)
			{
				this.Log("Source added: {0}", source.GetType().Name);
			}

			foreach (IResourceSource source in oldSources)
			{
				if (source.IsInitialized)
				{
					source.Unload();
				}

				this.Log("Source removed: {0}", source.GetType().Name);
			}

			foreach (IResourceSource source in currentSources)
			{
				if (!source.IsInitialized)
				{
					source.Initialize();
				}
			}

			this.UpdateAvailable();
		}

		#endregion




		#region Interface: IImporting

		/// <inheritdoc />
		void IImporting.ImportsResolved (CompositionFlags composition, bool updated)
		{
			if (updated)
			{
				this.UpdateConverters();
				this.UpdateSources();
			}
		}

		/// <inheritdoc />
		void IImporting.ImportsResolving (CompositionFlags composition)
		{
		}

		#endregion




		#region Interface: IResourceService

		/// <inheritdoc />
		public IEnumerable<string> AvailableResources
		{
			get
			{
				//TODO: Distinct
				foreach (IResourceSet set in this.AvailableSets)
				{
					foreach (string resource in set.AvailableResources)
					{
						yield return resource;
					}
				}
			}
		}

		/// <inheritdoc />
		public IEnumerable<IResourceSet> AvailableSets
		{
			get
			{
				foreach (IResourceSource source in this.Sources)
				{
					foreach (IResourceSet set in source.AvailableSets)
					{
						yield return set;
					}
				}
			}
		}


		/// <inheritdoc />
		public IEnumerable<IResourceConverter> Converters
		{
			get
			{
				foreach (IResourceConverter converter in this.ConvertersManual)
				{
					yield return converter;
				}

				foreach (IResourceConverter converter in this.ConvertersImported.Values<IResourceConverter>())
				{
					yield return converter;
				}
			}
		}

		/// <inheritdoc />
		public IEnumerable<string> LoadedResources
		{
			get
			{
				//TODO: Distinct
				foreach (IResourceSet set in this.LoadedSets)
				{
					foreach (string resource in set.AvailableResources)
					{
						yield return resource;
					}
				}
			}
		}

		/// <inheritdoc />
		public IEnumerable<IResourceSet> LoadedSets
		{
			get
			{
				foreach (IResourceSource source in this.Sources)
				{
					foreach (IResourceSet set in source.AvailableSets)
					{
						if (set.IsLoaded)
						{
							yield return set;
						}
					}
				}
			}
		}

		/// <inheritdoc />
		public IEnumerable<IResourceSource> Sources
		{
			get
			{
				foreach (IResourceSource storage in this.SourcesManual)
				{
					yield return storage;
				}

				foreach (IResourceSource storage in this.SourcesImported.Values<IResourceSource>())
				{
					yield return storage;
				}
			}
		}

		/// <inheritdoc />
		public void AddConverter (IResourceConverter resourceConverter)
		{
			if (resourceConverter == null)
			{
				throw new ArgumentNullException(nameof(resourceConverter));
			}

			if (this.ConvertersManual.Contains(resourceConverter))
			{
				return;
			}

			this.ConvertersManual.Add(resourceConverter);

			this.UpdateConverters();
		}

		/// <inheritdoc />
		public void AddSource (IResourceSource resourceSource)
		{
			if (resourceSource == null)
			{
				throw new ArgumentNullException(nameof(resourceSource));
			}

			if (this.SourcesManual.Contains(resourceSource))
			{
				return;
			}

			this.SourcesManual.Add(resourceSource);

			this.UpdateSources();
		}

		/// <inheritdoc />
		public object GetRawValue (string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			//TODO: Respect priority
			foreach (IResourceSet set in this.LoadedSets)
			{
				object value = set.GetRawValue(name);
				if (value != null)
				{
					return value;
				}
			}

			return null;
		}

		/// <inheritdoc />
		public T GetValue <T> (string name)
		{
			return (T)this.GetValue(name, typeof(T));
		}

		/// <inheritdoc />
		public object GetValue (string name, Type type)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			object value = this.GetRawValue(name);
			if (value == null)
			{
				return null;
			}

			IResourceConverter converter = this.GetConverterForType(value.GetType(), type);
			if (converter == null)
			{
				throw new InvalidTypeArgumentException();
			}

			object finalValue = converter.Convert(type, value);
			return finalValue;
		}

		/// <inheritdoc />
		public bool HasValue (string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			return Enumerable.Contains(this.LoadedResources, name, StringComparerEx.InvariantCultureIgnoreCase);
		}

		/// <inheritdoc />
		public void ReloadSets ()
		{
			this.UpdateAvailable();
			foreach (IResourceSet set in this.LoadedSets)
			{
				set.Load(set.IsLazyLoaded);
			}
		}

		/// <inheritdoc />
		public void RemoveConverter (IResourceConverter resourceConverter)
		{
			if (resourceConverter == null)
			{
				throw new ArgumentNullException(nameof(resourceConverter));
			}

			if (!this.ConvertersManual.Contains(resourceConverter))
			{
				return;
			}

			this.ConvertersManual.RemoveAll(resourceConverter);

			this.UpdateConverters();
		}

		/// <inheritdoc />
		public void RemoveSource (IResourceSource resourceSource)
		{
			if (resourceSource == null)
			{
				throw new ArgumentNullException(nameof(resourceSource));
			}

			if (!this.SourcesManual.Contains(resourceSource))
			{
				return;
			}

			this.SourcesManual.RemoveAll(resourceSource);

			this.UpdateSources();
		}

		/// <inheritdoc />
		public void UpdateAvailable ()
		{
			foreach (IResourceSource source in this.Sources)
			{
				source.UpdateAvailable();
			}

			foreach (IResourceSet set in this.AvailableSets)
			{
				set.UpdateAvailable();
			}
		}

		#endregion
	}
}
