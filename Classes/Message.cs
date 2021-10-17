using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Остатки.Classes
{
	public class Message 
	{
		public static List<string> errorsList = new List<string>();
		public static List<string> infoList = new List<string>();
		public static async void ShowInfo(object data)
		{
			ContentDialog errorDialog = new ContentDialog()
			{
				Title = "Успешно",
				Content = data.ToString(),
				PrimaryButtonText = "ОК"
			};
			await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
								() =>
								{
									var errorDialogRusult = errorDialog.ShowAsync();
								});
		}
		public static async void ShowInfoProduct(string name, object data)
		{
			ContentDialog InfoProductDialog = new ContentDialog()
			{
				Title = name,
				Content = data.ToString(),
				PrimaryButtonText = "ОК"
			};
			await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
								() =>
								{
									var InfoProductRusult = InfoProductDialog.ShowAsync();
								});
		}
		public static void ShowError(object data)
		{
			ContentDialog errorDialog = new ContentDialog()
			{
				Title = "Ошибка",
				Content = data.ToString(),
				PrimaryButtonText = "ОК"
			};
			var errorDialogRusult = errorDialog.ShowAsync();
		}
		public static void AllErrors()
		{
			if (errorsList.Count != 0)
			{
				string data = "";
				foreach (var item in errorsList)
				{
					data += item + "\n";
				}
				ShowError(data);
				errorsList.Clear();
			}else
			if (infoList.Count != 0)
			{
				string data = "";
				foreach (var item in infoList)
				{
					data += item + "\n";
				}
				ShowInfo(data);
				infoList.Clear();
			}
		}
	}
}
