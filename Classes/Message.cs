using Microsoft.Toolkit.Uwp.Notifications;
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
		public static void ShowInfo(object data)
		{
			ContentDialog errorDialog = new ContentDialog()
			{
				Title = "Успешно",
				Content = data.ToString(),
				PrimaryButtonText = "ОК"
			};
			var errorDialogRusult = errorDialog.ShowAsync();

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
		public static async void AllErrors()
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
				await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
								() =>
								{
									ShowInfo(data);
								});
				infoList.Clear();
			}
		}

		public static void ShowAllToast()
		{
			if (errorsList.Count != 0)
			{
				string data = "";
				foreach (var item in errorsList)
				{
					data += item + "\n";
				}
				errorsList.Clear();
				new ToastContentBuilder()
	.AddArgument("action", "viewConversation")
	.AddArgument("conversationId", 9813)
	.AddText("Ошибка!")
	.AddText(data)
	.Show();
			}
			else
			if (infoList.Count != 0)
			{
				string data = "";
				foreach (var item in infoList)
				{
					data += item + "\n";
				}
				infoList.Clear();
				new ToastContentBuilder()
	.AddArgument("action", "viewConversation")
	.AddArgument("conversationId", 9813)
	.AddText("Успех!")
	.AddText(data)
	.Show();
			}

		}
	}
}
