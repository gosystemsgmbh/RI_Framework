using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using RI.Framework.Collections;




namespace RI.Framework.Data.EF.Validation
{
	public sealed class ValidationRegistrar
	{
		#region Instance Constructor/Destructor

		internal ValidationRegistrar (IList<IEntityValidation> list)
		{
			if (list == null)
			{
				throw new ArgumentNullException(nameof(list));
			}

			this.List = list;
		}

		#endregion




		#region Instance Properties/Indexer

		private IList<IEntityValidation> List { get; set; }

		#endregion




		#region Instance Methods

		public void Add (IEntityValidation entityTypeValidator)
		{
			if (entityTypeValidator == null)
			{
				throw new ArgumentNullException(nameof(entityTypeValidator));
			}

			this.List.Add(entityTypeValidator);
		}

		public void AddFromAssembly (Assembly assembly)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException(nameof(assembly));
			}

			List<IEntityValidation> validators = (from x in assembly.GetTypes() where typeof(IEntityValidation).IsAssignableFrom(x) select (IEntityValidation)Activator.CreateInstance(x)).ToList();
			this.List.AddRange(validators);
		}

		#endregion
	}
}
