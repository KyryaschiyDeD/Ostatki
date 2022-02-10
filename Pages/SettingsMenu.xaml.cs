using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Остатки.Pages.SettingPages;
using Остатки.Pages.SettinPages;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace Остатки.Pages
{
	/// <summary>
	/// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
	/// </summary>
	public sealed partial class SettingsMenu : Page
	{
		public SettingsMenu()
		{
			this.InitializeComponent();
		}
		private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (AddWhiteOrBlackStore.IsSelected)
			{
				myFrame.Navigate(typeof(AddWhiteOrBlackStore));
				TitleTextBlock.Text = "Белый/Чёрный список";
			}
			else if (ManagementApiKeys.IsSelected)
			{
				myFrame.Navigate(typeof(ManagementApiKeys));
				TitleTextBlock.Text = "Управление API ключами";
			}
			else if (AddUnicArticle.IsSelected)
			{
				myFrame.Navigate(typeof(AddUnicArticle));
				TitleTextBlock.Text = "Управление артикулами";
			}
			else if (HostsPage.IsSelected)
			{
				myFrame.Navigate(typeof(HostsPage));
				TitleTextBlock.Text = "Хостинги";
			}
			else if (DelProductsToArchive.IsSelected)
			{
				myFrame.Navigate(typeof(DelProductsToArchive));
				TitleTextBlock.Text = "Убрать товары в архив файлом";
			}
		}

		private void HamburgerButton_Click(object sender, RoutedEventArgs e)
		{
			mySplitView.IsPaneOpen = !mySplitView.IsPaneOpen;
		}
	}
}
