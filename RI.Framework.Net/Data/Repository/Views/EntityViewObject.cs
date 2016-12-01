using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RI.Framework.Data.Repository.Views
{
    public class EntityViewObject<T>
        where T : class
    {
        public T Entity { get; internal set; }

        internal IEntityViewCaller ViewCaller { get; set; }
    }
}
