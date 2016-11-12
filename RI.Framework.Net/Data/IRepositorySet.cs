using System.Collections.Generic;
using System.Linq;




namespace RI.Framework.Data
{
	public interface IRepositorySet<T>
		where T : class
	{
		T Create();

		void Add(T entity);

		void Delete(T entity);

		void Reload(T entity);

		void Modify(T entity);

		IQueryable<T> GetQuery();

		IEnumerable<T> GetAll();

		IEnumerable<T> GetFiltered(string filter, int pageIndex, int pageSize, out int entityCount, out int pageCount);

		bool CanAdd(T entity);

		bool CanDelete(T entity);

		bool CanReload(T entity);

		bool CanModify(T entity);

		bool IsModified(T entity);

		IRepositoryErrors Validate(T entity);
	}
}