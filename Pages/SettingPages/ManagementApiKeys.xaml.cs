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
		}

		private void AddNewApiKey_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{
			ApiKeysesJob.CreateNewApi(NameInBase.Text, ClientId.Text, APIKey.Text);
			ApiList = new ObservableCollection<ApiKeys>(ApiKeysesJob.GetAllApiList());
		}
	}
}
