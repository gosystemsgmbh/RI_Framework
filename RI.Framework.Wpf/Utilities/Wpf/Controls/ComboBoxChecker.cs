using System.Linq;
using System.Windows;
using System.Windows.Controls;




namespace RI.Framework.Utilities.Wpf.Controls
{
	public class ComboBoxChecker : CheckBox
	{
		public static readonly DependencyProperty ComboBoxProperty = DependencyProperty.Register(nameof(ComboBoxChecker.ComboBox), typeof(ComboBox), typeof(ComboBoxChecker), new FrameworkPropertyMetadata(ComboBoxChecker.OnComboBoxChanged));

		public ComboBox ComboBox
		{
			get
			{
				return (System.Windows.Controls.ComboBox)this.GetValue(ComboBoxChecker.ComboBoxProperty);
			}
			set
			{
				this.SetValue(ComboBoxChecker.ComboBoxProperty, value);
			}
		}

		protected override void OnChecked (RoutedEventArgs e)
		{
			base.OnChecked(e);

			if (this.ComboBox == null)
			{
				return;
			}

			if (this.ComboBox.SelectedItem == null)
			{
				if (this.ComboBox.ItemsSource != null)
				{
					this.ComboBox.SelectedItem = this.ComboBox.ItemsSource.Cast<object>().FirstOrDefault();
				}
				else if(this.ComboBox.Items != null)
				{
					this.ComboBox.SelectedItem = this.ComboBox.Items.Cast<object>().FirstOrDefault();
				}
			}
		}

		protected override void OnUnchecked (RoutedEventArgs e)
		{
			base.OnUnchecked(e);

			if (this.ComboBox == null)
			{
				return;
			}

			if (this.ComboBox.SelectedItem != null)
			{
				this.ComboBox.SelectedItem = null;
			}
		}

		public ComboBoxChecker ()
		{
			this.SelectionChangedHandler = this.SelectionChangedMethod;
		}

		private SelectionChangedEventHandler SelectionChangedHandler { get; set; }

		private void SelectionChangedMethod(object sender, SelectionChangedEventArgs e)
		{
			this.UpdateChecked();
		}

		private void UnbindEvents ()
		{
			if (this.ComboBox != null)
			{
				this.ComboBox.SelectionChanged -= SelectionChangedHandler;
			}

			this.UpdateChecked();
		}

		private void BindEvents ()
		{
			if (this.ComboBox != null)
			{
				this.ComboBox.SelectionChanged += SelectionChangedHandler;
			}

			this.UpdateChecked();
		}

		private void UpdateChecked ()
		{
			this.IsChecked = this.ComboBox?.SelectedItem != null;
		}

		private static void OnComboBoxChanged (DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			((ComboBoxChecker)obj).UnbindEvents();
			((ComboBoxChecker)obj).BindEvents();
		}
	}
}