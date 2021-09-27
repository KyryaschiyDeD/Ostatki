using LiteDB;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Остатки.Classes;
using Остатки.Pages;

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
            Global.GetWhiteBlackShops();
            //List<Product> allProducts = new List<Product>();
            //
            //using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
            //{
            //    var col = db.GetCollection<Product>("Products");
            //    allProducts = col.Query().OrderBy(x => x.RemainsWhite).ToList();
            //}
            //List<Product> allProducts0 = allProducts.Where(x => x.RemainsWhite == 0).ToList();
            //List<Product> allProducts15 = allProducts.Where(x => x.RemainsWhite < 15).ToList();
            //remains0.Text = allProducts0.Count().ToString();
            //remains15.Text = allProducts15.Count().ToString();
             myFrame.Navigate(typeof(remains2));
            TitleTextBlock.Text = "Остатки в 3D";
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (remains.IsSelected)
            {
                myFrame.Navigate(typeof(remains2));
                TitleTextBlock.Text = "Остатки в 3D";
            }
            else if (add.IsSelected)
            {
                myFrame.Navigate(typeof(add));
                TitleTextBlock.Text = "Добавить";
            }
            else if (viewAll.IsSelected)
            {
                myFrame.Navigate(typeof(Settings));
                TitleTextBlock.Text = "Настройки";
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
            else if (spizdiliRemains.IsSelected)
            {
                myFrame.Navigate(typeof(StealProducts));
                TitleTextBlock.Text = "Спиздили";
            }
            else if (EditingBalancesAndPrices.IsSelected)
            {
                myFrame.Navigate(typeof(EditingBalancesAndPrices));
                TitleTextBlock.Text = "Обновление остатков и цены";
            }
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            mySplitView.IsPaneOpen = !mySplitView.IsPaneOpen;
        }
    }
}
