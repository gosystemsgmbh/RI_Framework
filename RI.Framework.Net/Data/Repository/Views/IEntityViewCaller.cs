namespace RI.Framework.Data.Repository.Views
{
	internal interface IEntityViewCaller <in TEntity>
		where TEntity : class
	{
		void Add (TEntity entity);

		void Attach (TEntity entity);

		void BeginEdit (TEntity entity);

		bool CanAdd (TEntity entity);

		bool CanAttach (TEntity entity);

		void CancelEdit (TEntity entity);

		bool CanDelete (TEntity entity);

		bool CanEdit (TEntity entity);

		bool CanModify (TEntity entity);

		bool CanReload (TEntity entity);

		bool CanSelect (TEntity entity);

		bool CanValidate (TEntity entity);

		void Delete (TEntity entity);

		void Deselect (TEntity entity);

		void EndEdit (TEntity entity);

		void Modify (TEntity entity);

		void Reload (TEntity entity);

		void Select (TEntity entity);

		void Validate (TEntity entity);
	}
}
