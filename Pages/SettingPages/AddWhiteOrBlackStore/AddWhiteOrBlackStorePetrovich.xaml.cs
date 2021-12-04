using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Остатки.Classes;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace Остатки.Pages.SettingPages.AddWhiteOrBlackStore
{
	/// <summary>
	/// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
	/// </summary>
	public sealed partial class AddWhiteOrBlackStorePetrovich : Page
	{
		public AddWhiteOrBlackStorePetrovich()
		{
			this.InitializeComponent();
			GoToShopList();
		}

		public ObservableCollection<ShopWhiteOrBlack> ShopList = new ObservableCollection<ShopWhiteOrBlack>();

		private void GoToShopList()
		{
			ShopList = new ObservableCollection<ShopWhiteOrBlack>(ShopWhiteOrBlackJob.GetShopListSpecifically("Петрович").OrderBy(x => x.Code));
			CountOfshops.Text = $"Количество магазинов: {ShopList.Count}";
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
			ShopWhiteOrBlackJob.CreateNewShop(ShopName.Text, ShopCode.Text, (bool)WhiteRadioButton.IsChecked, (bool)ShopIsOnlyThisCheckBox.IsChecked, "Петрович");
			Zeroing();
			GoToShopList();
			this.InitializeComponent();
		}

		private void RedactOldShop_Click(object sender, RoutedEventArgs e)
		{
			ShopWhiteOrBlackJob.RedactOldShop(ShopName.Text, ShopCode.Text, (bool)WhiteRadioButton.IsChecked, (bool)ShopIsOnlyThisCheckBox.IsChecked, "Петрович");
			Zeroing();
			GoToShopList();
		}
	}
}
