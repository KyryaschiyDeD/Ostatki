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

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace Остатки.Pages
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class PostingsJobMenu : Page
    {
        public PostingsJobMenu()
        {
            this.InitializeComponent();
        }
		private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			/*if (TaxationJob.IsSelected)
			{
				myFrame.Navigate(typeof(TaxationJob));
				TitleTextBlock.Text = "Белый/Чёрный список";
			}
			else if (ManagementApiKeys.IsSelected)
			{
				myFrame.Navigate(typeof(ManagementApiKeys));
				TitleTextBlock.Text = "Управление API ключами";
			}*/

		}

		private void HamburgerButton_Click(object sender, RoutedEventArgs e)
		{
			mySplitView.IsPaneOpen = !mySplitView.IsPaneOpen;
		}
	}
}
