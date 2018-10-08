﻿using System;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using RI.Framework.Composition.Model;
using RI.Framework.Services.Resources.Internals;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;
using RI.Framework.Utilities.ObjectModel;


namespace RI.Framework.Services.Resources.Converters
{
    /// <summary>
    ///     Implements a resource converter which handles common WPF types.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The types supported by this resource converter are:
    ///         <see cref="ImageSource" />, <see cref="BitmapSource" />, <see cref="BitmapImage" />, <see cref="ResourceDictionary" />.
    ///     </para>
    ///     <para>
    ///         See <see cref="IResourceConverter" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="true" instance="true" />
    [Export]
    public sealed class WpfResourceConverter : IResourceConverter
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="WpfResourceConverter" />.
        /// </summary>
        public WpfResourceConverter()
        {
            this.SyncRoot = new object();
        }

        #endregion




        #region Interface: IResourceConverter

        /// <inheritdoc />
        bool ISynchronizable.IsSynchronized => true;

        /// <inheritdoc />
        public object SyncRoot { get; }

        /// <inheritdoc />
        public bool CanConvert (Type sourceType, Type targetType)
        {
            if (sourceType == null)
            {
                throw new ArgumentNullException(nameof(sourceType));
            }

            if (targetType == null)
            {
                throw new ArgumentNullException(nameof(targetType));
            }

            if ((sourceType == typeof(BitmapImage)) && ((targetType == typeof(ImageSource)) || (targetType == typeof(BitmapSource)) || (targetType == typeof(BitmapImage))))
            {
                return true;
            }

            if ((sourceType == typeof(BitmapSource)) && ((targetType == typeof(ImageSource)) || (targetType == typeof(BitmapSource))))
            {
                return true;
            }

            if ((sourceType == typeof(ImageSource)) && (targetType == typeof(ImageSource)))
            {
                return true;
            }

            if ((sourceType == typeof(ResourceDictionary)) && (targetType == typeof(ResourceDictionary)))
            {
                return true;
            }

            if ((sourceType == typeof(byte[])) && ((targetType == typeof(ImageSource)) || (targetType == typeof(BitmapSource)) || (targetType == typeof(BitmapImage))))
            {
                return true;
            }

            if ((sourceType == typeof(byte[])) && (targetType == typeof(ResourceDictionary)))
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public object Convert (Type type, object value)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if ((value is BitmapImage) && ((type == typeof(ImageSource)) || (type == typeof(BitmapSource)) || (type == typeof(BitmapImage))))
            {
                return value;
            }

            if ((value is BitmapSource) && ((type == typeof(ImageSource)) || (type == typeof(BitmapSource))))
            {
                return value;
            }

            if ((value is ImageSource) && (type == typeof(ImageSource)))
            {
                return value;
            }

            if ((value is ResourceDictionary) && (type == typeof(ResourceDictionary)))
            {
                return value;
            }

            if ((value is byte[]) && ((type == typeof(ImageSource)) || (type == typeof(BitmapSource)) || (type == typeof(BitmapImage))))
            {
                using (MemoryStream ms = new MemoryStream((byte[])value, false))
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.CreateOptions = BitmapCreateOptions.None;
                    image.StreamSource = ms;
                    image.EndInit();
                    return image.Clone();
                }
            }

            if ((value is byte[]) && (type == typeof(ResourceDictionary)))
            {
                using (MemoryStream ms = new MemoryStream((byte[])value, false))
                {
                    object xamlResult = XamlReader.Load(ms);
                    return (ResourceDictionary)xamlResult;
                }
            }

            throw new InvalidTypeArgumentException(nameof(value));
        }

        /// <inheritdoc />
        public ResourceLoadingInfo GetLoadingInfoFromFileExtension (string extension)
        {
            if (extension == null)
            {
                throw new ArgumentNullException(nameof(extension));
            }

            if (extension.IsEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(extension));
            }

            extension = extension.ToUpperInvariant().Trim();

            if (extension == ".PNG")
            {
                return new ResourceLoadingInfo(ResourceLoadingType.Binary, typeof(BitmapImage));
            }

            if (extension == ".XAML")
            {
                return new ResourceLoadingInfo(ResourceLoadingType.Binary, typeof(ResourceDictionary));
            }

            return null;
        }

        #endregion
    }
}
