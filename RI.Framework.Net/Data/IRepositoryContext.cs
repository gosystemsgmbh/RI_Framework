namespace RI.Framework.Data
{
	public interface IRepositoryContext
	{
		IRepositorySet<T> GetSet<T>() where T : class;

		bool HasChanges();

		bool HasErrors();

		void SaveChanges();

		void RollbackErrors();

		void Commit();

		void Rollback();
	}
}