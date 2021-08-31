using LiteDB;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Остатки.Classes;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace Остатки
{
	/// <summary>
	/// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
	/// </summary>
	public sealed partial class add : Page
	{
		Queue<string> ochered = new Queue<string>();
		StorageFile file;
		public add()
		{
			this.InitializeComponent();
		}

		public void UpdateProgress(double kolvo, double apply)
		{
			// Construct a NotificationData object;
			string tag = "Product";
			string group = "Lerya";

			// Create NotificationData and make sure the sequence number is incremented
			/* since last update, or assign 0 for updating regardless of order*/
			var data = new NotificationData
			{
				SequenceNumber = 0
			};

			// Assign new values
			// Note that you only need to assign values that changed. In this example
			// we don't assign progressStatus since we don't need to change it
			data.Values["myProgressValue"] = (apply / kolvo).ToString().Replace(",", ".");
			data.Values["progressValueString"] = $"{apply}/{kolvo} товаров";

			// Update the existing notification's data by using tag/group
			ToastNotificationManager.CreateToastNotifier().Update(data, tag, group);
		}

		private void addLink_Click(object sender, RoutedEventArgs e)
		{
			link.Text.Replace(" ", "");
			if (link.Text.Length != 0)
				ProductJobs.parseLerya(link.Text);
			else
				Message.errorsList.Add("Вы ввели пустую строку!");
			link.Text = "";
			Message.AllErrors();
		}

		async void addFileLinksThread()
		{
			string tag = "Product";
			string group = "Lerya";

			// Construct the toast content with data bound fields
			var content = new ToastContentBuilder()
				.AddText("Тсссс... Происходит анализ... Не мешай!")
				.AddVisualChild(new AdaptiveProgressBar()
				{
					Title = "Товар",
					Value = new BindableProgressBarValue("myProgressValue"),
					ValueStringOverride = new BindableString("progressValueString"),
					Status = new BindableString("progressStatus")
				})
				.GetToastContent();

			// Generate the toast notification
			var toast = new ToastNotification(content.GetXml());

			// Assign the tag and group
			toast.Tag = tag;
			toast.Group = group;

			// Assign initial NotificationData values
			// Values must be of type string
			toast.Data = new NotificationData();
			toast.Data.Values["progressValue"] = "0";
			toast.Data.Values["progressValueString"] = "0/0 товаров";
			toast.Data.Values["progressStatus"] = "Анализируем";

			// Provide sequence number to prevent out-of-order updates, or assign 0 to indicate "always update"
			toast.Data.SequenceNumber = 0;

			// Show the toast notification to the user
			ToastNotificationManager.CreateToastNotifier().Show(toast);
			//IList<string> linksProductTXT = await FileIO.ReadLinesAsync(file);
			ochered = new Queue<string>(await FileIO.ReadLinesAsync(file));
			/*foreach (var item in linksProductTXT)
			{
				ochered.Enqueue(item);
			} */
			Task[] tasks2 = new Task[ochered.Count];
			int allCount = ochered.Count;
			for (int i = 0; i < allCount; i++)
			{
				tasks2[i] = Task.Factory.StartNew(() => ProductJobs.parseLerya(ochered.Dequeue()));
				UpdateProgress(allCount, i+1);
			}
			//Task.WaitAll(tasks2);

		}

		private async void addFileLinks_Click(object sender, RoutedEventArgs e)
		{
			var picker = new Windows.Storage.Pickers.FileOpenPicker();
			picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
			picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
			picker.FileTypeFilter.Add(".txt");
			file = await picker.PickSingleFileAsync();
			if (file != null)
			{
				Thread thread = new Thread(addFileLinksThread);
				thread.Start();
			}
		}

		private async void createFileLinks_Click(object sender, RoutedEventArgs e)
		{
			string allLinks = "";
			using (var db = new LiteDatabase($@"{ApplicationData.Current.LocalFolder.Path}/ProductsDB.db"))
			{
				var col = db.GetCollection<Product>("Products");
				var allList = col.FindAll();
				foreach (var item in allList)
				{
					allLinks += item.ProductLink + "\n";
				}
			}
			var savePicker = new FileSavePicker();
			// место для сохранения по умолчанию
			savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
			// устанавливаем типы файлов для сохранения
			savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
			// устанавливаем имя нового файла по умолчанию
			savePicker.SuggestedFileName = "AllLinksBaseOzon";
			savePicker.CommitButtonText = "Сохранить";

			var new_file = await savePicker.PickSaveFileAsync();
			if (new_file != null)
			{
				await FileIO.WriteTextAsync(new_file, allLinks);
			}
		}

		private async void updateAllData_Click(object sender, RoutedEventArgs e)
		{
			string allLinks = "";
			using (var db = new LiteDatabase($@"{ApplicationData.Current.LocalFolder.Path}/ProductsDB.db"))
			{
				var col = db.GetCollection<Product>("Products");
				var allList = col.FindAll();
				foreach (var item in allList)
				{
					allLinks += item.ProductLink + "\n";
				}
			}
			var savePicker = new FileSavePicker();
			// место для сохранения по умолчанию
			savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
			// устанавливаем типы файлов для сохранения
			savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
			// устанавливаем имя нового файла по умолчанию
			savePicker.SuggestedFileName = "AllLinksBaseOzon";
			savePicker.CommitButtonText = "Сохранить";

			var new_file = await savePicker.PickSaveFileAsync();
			if (new_file != null)
			{
				await FileIO.WriteTextAsync(new_file, allLinks);
			}

			File.Delete($@"{ApplicationData.Current.LocalFolder.Path}/ProductsDB.db");
		}
	}
}
