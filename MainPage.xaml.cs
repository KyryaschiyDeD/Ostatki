using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
                myFrame.Navigate(typeof(viewAll));
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
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            mySplitView.IsPaneOpen = !mySplitView.IsPaneOpen;
        }
    }
}
