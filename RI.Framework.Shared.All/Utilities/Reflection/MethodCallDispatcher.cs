using System;
using System.Collections.Generic;

using RI.Framework.ComponentModel;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Utilities.Reflection
{
	//TODO: Equality check
	public sealed class MethodCallDispatcher
	{
		#region Static Properties/Indexer

		private static object GlobalSyncRoot { get; } = new object();

		private static Dictionary<Type, Dictionary<string, MethodCallDispatcher>> Prototypes { get; } = new Dictionary<Type, Dictionary<string, MethodCallDispatcher>>();

		#endregion




		#region Static Methods

		public static MethodCallDispatcher FromTarget (object target, string methodName)
		{
			if (target == null)
			{
				throw new ArgumentNullException(nameof(target));
			}

			if (methodName == null)
			{
				throw new ArgumentNullException(nameof(methodName));
			}

			if (methodName.IsNullOrEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(methodName));
			}

			return MethodCallDispatcher.CreateInternal(target, target.GetType(), methodName);
		}

		public static MethodCallDispatcher FromType (Type type, string methodName)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (methodName == null)
			{
				throw new ArgumentNullException(nameof(methodName));
			}

			if (methodName.IsNullOrEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(methodName));
			}

			return MethodCallDispatcher.CreateInternal(null, type, methodName);
		}

		private static MethodCallDispatcher CreateInternal (object target, Type type, string methodName)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			if (methodName == null)
			{
				throw new ArgumentNullException(nameof(methodName));
			}

			if (methodName.IsNullOrEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(methodName));
			}

			lock (MethodCallDispatcher.GlobalSyncRoot)
			{
				if (!MethodCallDispatcher.Prototypes.ContainsKey(type))
				{
					MethodCallDispatcher.Prototypes.Add(type, new Dictionary<string, MethodCallDispatcher>(StringComparerEx.Ordinal));
				}

				if (!MethodCallDispatcher.Prototypes[type].ContainsKey(methodName))
				{
					MethodCallDispatcher.Prototypes[type].Add(methodName, new MethodCallDispatcher(type, methodName));
				}

				MethodCallDispatcher prototype = MethodCallDispatcher.Prototypes[type][methodName];
				return target == null ? prototype : new MethodCallDispatcher(target, prototype);
			}
		}

		#endregion




		#region Instance Constructor/Destructor

		private MethodCallDispatcher (Type type, string methodName)
		{
			this.Type = type;
			this.MethodName = methodName;
			this.Target = null;

			this.Initialize(null);
		}

		private MethodCallDispatcher (object target, MethodCallDispatcher prototype)
		{
			this.Type = prototype.Type;
			this.MethodName = prototype.MethodName;
			this.Target = target;

			this.Initialize(prototype);
		}

		#endregion




		#region Instance Properties/Indexer

		public string MethodName { get; }

		public object Target { get; }

		public Type Type { get; }

		#endregion




		#region Instance Methods

		public object Dispatch (object parameter)
		{
			if (parameter == null)
			{
				throw new ArgumentNullException(nameof(parameter));
			}

			object returnValue = null;
			this.DispatchInternal(parameter, out returnValue, null);
			return returnValue;
		}

		public object Dispatch (object parameter, IDependencyResolver parameterResolver)
		{
			if (parameter == null)
			{
				throw new ArgumentNullException(nameof(parameter));
			}

			if (parameterResolver == null)
			{
				throw new ArgumentNullException(nameof(parameterResolver));
			}

			object returnValue = null;
			this.DispatchInternal(parameter, out returnValue, (name, type) => parameterResolver.GetInstance(type) ?? parameterResolver.GetInstance(name));
			return returnValue;
		}

		public object Dispatch (object parameter, Func<string, Type, object> parameterResolver)
		{
			if (parameter == null)
			{
				throw new ArgumentNullException(nameof(parameter));
			}

			if (parameterResolver == null)
			{
				throw new ArgumentNullException(nameof(parameterResolver));
			}

			object returnValue = null;
			this.DispatchInternal(parameter, out returnValue, parameterResolver);
			return returnValue;
		}

		public bool Dispatch (object parameter, out object returnValue)
		{
			if (parameter == null)
			{
				throw new ArgumentNullException(nameof(parameter));
			}

			return this.DispatchInternal(parameter, out returnValue, null);
		}

		public bool Dispatch (object parameter, out object returnValue, IDependencyResolver parameterResolver)
		{
			if (parameter == null)
			{
				throw new ArgumentNullException(nameof(parameter));
			}

			if (parameterResolver == null)
			{
				throw new ArgumentNullException(nameof(parameterResolver));
			}

			return this.DispatchInternal(parameter, out returnValue, (name, type) => parameterResolver.GetInstance(type) ?? parameterResolver.GetInstance(name));
		}

		public bool Dispatch (object parameter, out object returnValue, Func<string, Type, object> parameterResolver)
		{
			if (parameter == null)
			{
				throw new ArgumentNullException(nameof(parameter));
			}

			if (parameterResolver == null)
			{
				throw new ArgumentNullException(nameof(parameterResolver));
			}

			return this.DispatchInternal(parameter, out returnValue, parameterResolver);
		}

		public bool CanDispatchParameter (object parameter)
		{
			if (parameter == null)
			{
				throw new ArgumentNullException(nameof(parameter));
			}

			return this.CanDispatchType(parameter.GetType());
		}

		public bool CanDispatchType (Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			//TODO
			return false;
		}

		private bool DispatchInternal (object parameter, out object returnValue, Func<string, Type, object> parameterResolver)
		{
			parameterResolver = parameterResolver ?? ((name, type) => type.GetDefaultValue());

			//TODO
			returnValue = null;
			return false;
		}

		private void Initialize (MethodCallDispatcher prototype)
		{
			//TODO
		}

		#endregion
	}
}
