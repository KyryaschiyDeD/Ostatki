using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace Остатки.Pages.Statistics
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class PostingsJobPage : Page
    {
        public PostingsJobPage()
        {
            this.InitializeComponent();
        }
        private void GetPostings_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
