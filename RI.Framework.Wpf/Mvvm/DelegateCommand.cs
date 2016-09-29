using System;
using System.Windows.Input;




namespace RI.Framework.Mvvm
{
	public sealed class DelegateCommand<T> : ICommand
	{
		public bool CanExecute (object parameter)
		{
			throw new NotImplementedException();
		}

		public void Execute (object parameter)
		{
			throw new NotImplementedException();
		}

		public event EventHandler CanExecuteChanged;
	}
}