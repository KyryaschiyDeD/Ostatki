using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Остатки.Classes;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace Остатки.Pages.SettinPages
{
	/// <summary>
	/// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
	/// </summary>
	public sealed partial class AddWhiteOrBlackStore : Page
	{
		public ObservableCollection<ShopWhiteOrBlack> ShopList = new ObservableCollection<ShopWhiteOrBlack>();
		public AddWhiteOrBlackStore()
		{
			this.InitializeComponent();
			GoToShopList();
		}

		private void GoToShopList()
		{
			ShopList = new ObservableCollection<ShopWhiteOrBlack>(ShopWhiteOrBlackJob.GetAllShopList());
		}

		private void Zeroing()
		{
			ShopName.Text = "";
			ShopCode.Text = "";
			WhiteRadioButton.IsChecked = false;
			BlackRadioButton.IsChecked = false;
		}

		private void CreateNewShop_Click(object sender, RoutedEventArgs e)
		{
			ShopWhiteOrBlackJob.CreateNewShop(ShopName.Text, ShopCode.Text, (bool)WhiteRadioButton.IsChecked, (bool)ShopIsOnlyThisCheckBox.IsChecked);
			GoToShopList();
			Zeroing();
		}

		private void RedactOldShop_Click(object sender, RoutedEventArgs e)
		{
			ShopWhiteOrBlackJob.RedactOldShop(ShopName.Text, ShopCode.Text, (bool)WhiteRadioButton.IsChecked, (bool)ShopIsOnlyThisCheckBox.IsChecked);
			GoToShopList();
			Zeroing();
		}
	}
}
