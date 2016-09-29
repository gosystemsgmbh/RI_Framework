using System;
using System.Windows.Input;




namespace RI.Framework.Mvvm
{
	public sealed class DelegateCommand <T> : ICommand
	{
		#region Interface: ICommand

		public event EventHandler CanExecuteChanged;

		public bool CanExecute (object parameter)
		{
			throw new NotImplementedException();
		}

		public void Execute (object parameter)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
