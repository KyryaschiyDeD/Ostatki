using LiteDB;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Остатки.Classes;
using Остатки.Pages;
using Остатки.Pages.Statistics;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace Остатки
{
	/// <summary>
	/// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
	/// </summary>
	public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            Global.GetWhiteBlackShopsLeroy();
            Global.GetWhiteBlackShopsLeonardo();
            Global.WebHosting = WebHostingsJob.GetHostsList();
           
            //ApiKeysesJob.DeleteOneApi("181882");

            myFrame.Navigate(typeof(StatisticsMenu));
            TitleTextBlock.Text = "Статка";

        }


        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StatisticsMenu.IsSelected)
            {
                myFrame.Navigate(typeof(StatisticsMenu));
                TitleTextBlock.Text = "Статистика";
            }
            else if(remains.IsSelected)
            {
                myFrame.Navigate(typeof(RemainsMenu));
                TitleTextBlock.Text = "Остатки в 3D";
            }
            else if (add.IsSelected)
            {
                myFrame.Navigate(typeof(AddJobProductMenu));
                TitleTextBlock.Text = "Воровать и добавлять";
            }
            else if (settings.IsSelected)
            {
                myFrame.Navigate(typeof(SettingsMenu));
                TitleTextBlock.Text = "Настройки";
            }
            /*else if (EditingBalancesAndPrices.IsSelected)
            {
                myFrame.Navigate(typeof(EditingBalancesAndPrices));
                TitleTextBlock.Text = "Обновление остатков и цены";
            }*/
            else if (PostingsJobPage.IsSelected)
            {
                myFrame.Navigate(typeof(PostingsJobPage));
                TitleTextBlock.Text = "Работа с отправлениями";
            }
            else if (TaxationMenu.IsSelected)
            {
                myFrame.Navigate(typeof(TaxationMenu));
                TitleTextBlock.Text = "Налоги";
            }
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            mySplitView.IsPaneOpen = !mySplitView.IsPaneOpen;
        }
    }
}
