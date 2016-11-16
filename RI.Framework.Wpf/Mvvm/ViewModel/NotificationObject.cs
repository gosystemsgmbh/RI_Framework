using System.ComponentModel;




namespace RI.Framework.Mvvm.ViewModel
{
	/// <summary>
	///     Implements a base class which can raise notifications about changing its state and data.
	/// </summary>
	public abstract class NotificationObject : INotifyPropertyChanged, INotifyPropertyChanging
	{
		#region Virtuals

		/// <summary>
		///     Handles the change of a property value by raising the <see cref="PropertyChanged" /> event.
		/// </summary>
		/// <param name="propertyName"> The name of the property which has changed. </param>
		protected virtual void OnPropertyChanged (string propertyName)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}



		/// <summary>
		///     Handles the change of a property value by raising the <see cref="PropertyChanging" /> event.
		/// </summary>
		/// <param name="propertyName"> The name of the property which is about to be changed. </param>
		protected virtual void OnPropertyChanging(string propertyName)
		{
			this.PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
		}

		#endregion




		#region Interface: INotifyPropertyChanged

		/// <inheritdoc />
		public event PropertyChangedEventHandler PropertyChanged;

		/// <inheritdoc />
		public event PropertyChangingEventHandler PropertyChanging;

		#endregion
	}
}
