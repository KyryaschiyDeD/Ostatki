using Microsoft.Toolkit.Uwp.UI.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using Остатки.Classes;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace Остатки.Pages.SettinPages
{
	/// <summary>
	/// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
	/// </summary>
	public sealed partial class ManagementApiKeys : Page
	{
		public ObservableCollection<ApiKeys> ApiList = new ObservableCollection<ApiKeys>();
		public ManagementApiKeys()
		{
			this.InitializeComponent();
			ApiList = new ObservableCollection<ApiKeys>(ApiKeysesJob.GetAllApiList());
			//dataGridWhiteOrBlackShop.DataContext = ApiList;
		}

		private void AddNewApiKey_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{
			ApiKeysesJob.CreateNewApi
				(
				NameInBase.Text,
				ClientId.Text,
				APIKey.Text,
				MaxCountTopProduct.Text,
				ItIsTop.IsChecked,
				InDB.IsChecked,
				IsOstatkiUpdate.IsChecked,
				IsPriceUpdate.IsChecked,
				IsTheMaximumPrice.IsChecked
				);
			ApiList = new ObservableCollection<ApiKeys>(ApiKeysesJob.GetAllApiList());
		}

		private void ReadctOldApiKey_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{
			ApiKeysesJob.ReadctOldApi
				(
				NameInBase.Text, 
				ClientId.Text, 
				APIKey.Text
				);
			ApiList = new ObservableCollection<ApiKeys>(ApiKeysesJob.GetAllApiList());
		}
		private void SaveUpdate_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{
			List<ApiKeys> apiKeysOld = new List<ApiKeys>(ApiKeysesJob.GetAllApiList());
			List<ApiKeys> apiKeysNew = new List<ApiKeys>();
            foreach (var item in ApiList)
            {
				apiKeysNew.Add(item);
			}
            foreach (var newApi in apiKeysNew)
            {
				if (newApi != apiKeysOld.Find(x => x.Id == newApi.Id))
                {
					ApiKeysesJob.UpdateOneApi(newApi);
				}
            }
			SaveUpdate.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
		}
		private void dataGridWhiteOrBlackShop_BeginningEdit( object sender, DataGridBeginningEditEventArgs e)
		{
			SaveUpdate.Visibility = Windows.UI.Xaml.Visibility.Visible;
		}
	}
}
