using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Остатки.Classes;
using Остатки.Pages.SettingPages.AddWhiteOrBlackStore;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace Остатки.Pages.SettinPages
{
	/// <summary>
	/// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
	/// </summary>
	public sealed partial class AddWhiteOrBlackStore : Page
	{
		public AddWhiteOrBlackStore()
		{
			this.InitializeComponent();
		}

		private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (Leroy.IsSelected)
			{
				myFrame.Navigate(typeof(AddWhiteOrBlackStoreLeroy));
				TitleTextBlock.Text = "Леруа";
			}
			else if (Leonardo.IsSelected)
			{
				myFrame.Navigate(typeof(AddWhiteOrBlackStoreLeonardo));
				TitleTextBlock.Text = "Леонардо";
			}
			else if (Petrovich.IsSelected)
			{
				myFrame.Navigate(typeof(AddWhiteOrBlackStorePetrovich));
				TitleTextBlock.Text = "Петрович";
			}
		}

		private void HamburgerButton_Click(object sender, RoutedEventArgs e)
		{
			mySplitView.IsPaneOpen = !mySplitView.IsPaneOpen;
		}
	}
}
