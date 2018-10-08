﻿using System;
using System.Collections.Generic;

using RI.Framework.Collections;
using RI.Framework.Collections.DirectLinq;
using RI.Framework.Composition;
using RI.Framework.Composition.Model;
using RI.Framework.Services.Resources.Converters;
using RI.Framework.Services.Resources.Sources;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.Logging;




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
	[Export]
	public sealed class ResourceService : LogSource, IResourceService, IImporting
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

		[Import(typeof(IResourceConverter), Recomposable = true)]
		private Import ConvertersImported { get; set; }

		private List<IResourceConverter> ConvertersManual { get; set; }

		private List<IResourceConverter> ConvertersUpdated { get; set; }

		[Import(typeof(IResourceSource), Recomposable = true)]
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

		private void UpdateConverters ()
		{
			this.Log(LogLevel.Debug, "Updating converters");

			HashSet<IResourceConverter> currentConverters = new HashSet<IResourceConverter>(this.Converters);
			HashSet<IResourceConverter> lastConverters = new HashSet<IResourceConverter>(this.ConvertersUpdated);

			HashSet<IResourceConverter> newConverters = currentConverters.Except(lastConverters);
			HashSet<IResourceConverter> oldConverters = lastConverters.Except(currentConverters);

			this.ConvertersUpdated.Clear();
			this.ConvertersUpdated.AddRange(currentConverters);

			foreach (IResourceConverter converter in newConverters)
			{
				this.Log(LogLevel.Debug, "Converter added: {0}", converter.GetType().Name);
			}

			foreach (IResourceConverter converter in oldConverters)
			{
				this.Log(LogLevel.Debug, "Converter removed: {0}", converter.GetType().Name);
			}

			foreach (IResourceSource source in this.Sources)
			{
				source.UpdateConverters(this.Converters);
			}
		}

		private void UpdateSources ()
		{
			this.Log(LogLevel.Debug, "Updating sources");

			HashSet<IResourceSource> currentSources = new HashSet<IResourceSource>(this.Sources);
			HashSet<IResourceSource> lastSources = new HashSet<IResourceSource>(this.SourcesUpdated);

			HashSet<IResourceSource> newSources = currentSources.Except(lastSources);
			HashSet<IResourceSource> oldSources = lastSources.Except(currentSources);

			this.SourcesUpdated.Clear();
			this.SourcesUpdated.AddRange(currentSources);

			foreach (IResourceSource source in newSources)
			{
				this.Log(LogLevel.Debug, "Source added: {0}", source.GetType().Name);
			}

			foreach (IResourceSource source in oldSources)
			{
				source.Unload();

				this.Log(LogLevel.Debug, "Source removed: {0}", source.GetType().Name);
			}

			foreach (IResourceSource source in currentSources)
			{
				if (!source.IsInitialized)
				{
					source.Initialize(this.Converters);
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
		public HashSet<string> GetAvailableResources ()
		{
			HashSet<string> resources = new HashSet<string>(StringComparerEx.InvariantCultureIgnoreCase);
			foreach (IResourceSet set in this.GetLoadedSets())
			{
				resources.AddRange(set.GetAvailableResources());
			}
			return resources;
		}

		/// <inheritdoc />
		public List<IResourceSet> GetAvailableSets ()
		{
			List<IResourceSet> sets = new List<IResourceSet>();
			foreach (IResourceSource source in this.Sources)
			{
				sets.AddRange(source.GetAvailableSets());
			}
			return sets;
		}

		/// <inheritdoc />
		public List<IResourceSet> GetLoadedSets () => new List<IResourceSet>(from x in this.GetAvailableSets() where x.IsLoaded select x);

		/// <inheritdoc />
		public object GetRawValue (string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			foreach (IResourceSet set in this.GetLoadedSets())
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
		public T GetValue <T> (string name) => (T)this.GetValue(name, typeof(T));

		/// <inheritdoc />
		public object GetValue (string name, Type type)
		{
			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsEmptyOrWhitespace())
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
				throw new InvalidTypeArgumentException(nameof(type));
			}

			object finalValue = converter.Convert(type, value);
			return finalValue;
		}

		/// <inheritdoc />
		public void LoadSet (IResourceSet resourceSet, bool lazyLoad)
		{
			if (resourceSet == null)
			{
				throw new ArgumentNullException(nameof(resourceSet));
			}

			this.Log(LogLevel.Debug, "Loading set: {0}", resourceSet.Id);

			resourceSet.Load(lazyLoad);
		}

		/// <inheritdoc />
		public void ReloadSets ()
		{
			this.Log(LogLevel.Debug, "Reloading all loaded sets");

			foreach (IResourceSet set in this.GetLoadedSets())
			{
				this.LoadSet(set, set.IsLazyLoaded);
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
		public void UnloadSet (IResourceSet resourceSet)
		{
			if (resourceSet == null)
			{
				throw new ArgumentNullException(nameof(resourceSet));
			}

			this.Log(LogLevel.Debug, "Unlaoding set: {0}", resourceSet.Id);

			resourceSet.Unload();
		}

		/// <inheritdoc />
		public void UnloadSets ()
		{
			this.Log(LogLevel.Debug, "Unloading all loaded sets");

			foreach (IResourceSet set in this.GetLoadedSets())
			{
				this.UnloadSet(set);
			}
		}

		#endregion
	}
}
