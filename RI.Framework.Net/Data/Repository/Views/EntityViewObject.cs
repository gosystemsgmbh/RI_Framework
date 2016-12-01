using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RI.Framework.Data.Repository.Views
{
    public class EntityViewObject<TEntity>
        where TEntity : class
    {
        public TEntity Entity { get; internal set; }

        internal IEntityViewCaller<TEntity> ViewCaller { get; set; }
    }
}
