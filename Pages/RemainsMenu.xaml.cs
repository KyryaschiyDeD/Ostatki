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
using Остатки.Pages.RemainsPages;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace Остатки.Pages
{
	/// <summary>
	/// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
	/// </summary>
	public sealed partial class RemainsMenu : Page
	{
		public RemainsMenu()
		{
			this.InitializeComponent();
			myFrame.Navigate(typeof(remains2));
		}

		private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (remains.IsSelected)
			{
				myFrame.Navigate(typeof(remains2));
				TitleTextBlock.Text = "Остатки";
			}
			else if (waitRemains.IsSelected)
			{
				myFrame.Navigate(typeof(waitRemains));
				TitleTextBlock.Text = "Ждём появления";
			}
			else if (archiveRemains.IsSelected)
			{
				myFrame.Navigate(typeof(archiveRemains));
				TitleTextBlock.Text = "Архив :-(";
			}
			else if (ExaminationYandex.IsSelected)
			{
				myFrame.Navigate(typeof(ExaminationYandex));
				TitleTextBlock.Text = "Проверить яндекс!";
			}
			else if (ProductsFromMarket.IsSelected)
			{
				myFrame.Navigate(typeof(ProductFromMArket));
				TitleTextBlock.Text = "Продукты с маркета";
			}
			else if (ErrorsRemains.IsSelected)
			{
				myFrame.Navigate(typeof(errorsRemains));
				TitleTextBlock.Text = "Продукты с ошибкой";
			}
		}

		private void HamburgerButton_Click(object sender, RoutedEventArgs e)
		{
			mySplitView.IsPaneOpen = !mySplitView.IsPaneOpen;
		}
	}
}
