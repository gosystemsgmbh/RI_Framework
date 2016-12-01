namespace RI.Framework.Data.Repository.Views
{
	internal interface IEntityViewCaller<in TEntity>
		where TEntity : class
	{
		void Attach(TEntity entity);

		void Add(TEntity entity);

		void Delete (TEntity entity);

		void BeginEdit (TEntity entity);

		void EndEdit(TEntity entity);

		void CancelEdit(TEntity entity);

		void Reload(TEntity entity);

		void Modify(TEntity entity);

		void Validate(TEntity entity);

		bool CanAdd(TEntity entity);

		bool CanAttach(TEntity entity);

		bool CanDelete(TEntity entity);

		bool CanEdit(TEntity entity);

		bool CanReload(TEntity entity);

		bool CanModify(TEntity entity);

		bool CanValidate(TEntity entity);
	}
}