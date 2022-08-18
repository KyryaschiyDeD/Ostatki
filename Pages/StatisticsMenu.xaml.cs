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
using Остатки.Pages.Statistics;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace Остатки.Pages
{
	/// <summary>
	/// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
	/// </summary>
	public sealed partial class StatisticsMenu : Page
	{
		public StatisticsMenu()
		{
            this.InitializeComponent();
            myFrame.Navigate(typeof(MainInfoList));
            TitleTextBlock.Text = "Статка";
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainInfoList.IsSelected)
            {
                myFrame.Navigate(typeof(MainInfoList));
                TitleTextBlock.Text = "Статистика";
            }
            else if (TestFinansy.IsSelected)
            {
                myFrame.Navigate(typeof(TestFinansy));
                TitleTextBlock.Text = "Финансы тест";
            }
            else if (bestStore.IsSelected)
            {
                myFrame.Navigate(typeof(BestStore));
                TitleTextBlock.Text = "Лучшие продажи гена";
            }
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            mySplitView.IsPaneOpen = !mySplitView.IsPaneOpen;
        }
    }
}
