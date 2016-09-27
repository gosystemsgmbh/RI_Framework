using System;
using System.Windows.Markup;

using RI.Framework.Services;
using RI.Framework.Utilities;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Mvvm
{
	[MarkupExtensionReturnType (typeof(object))]
	public sealed class ObjectLocator : MarkupExtension
	{
		#region Static Methods

		internal static object GetValue (string name)
		{
			if (name == null)
			{
				return null;
			}

			if (name.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			object value = ServiceLocator.GetInstance(name);
			ObjectLocator.ProcessValue(value);
			return value;
		}

		internal static object GetValue (Type type)
		{
			if (type == null)
			{
				return null;
			}

			object value = ServiceLocator.GetInstance(type);
			ObjectLocator.ProcessValue(value);
			return value;
		}

		internal static void ProcessValue (object value)
		{
			//TODO
		}

		#endregion




		#region Instance Constructor/Destructor

		public ObjectLocator ()
		{
			this.Name = null;
			this.Type = null;
		}

		#endregion




		#region Instance Properties/Indexer

		public string Name { get; set; }

		public Type Type { get; set; }

		#endregion




		#region Overrides

		public override object ProvideValue (IServiceProvider serviceProvider)
		{
			return ObjectLocator.GetValue(this.Name) ?? ObjectLocator.GetValue(this.Type);
		}

		#endregion
	}
}
