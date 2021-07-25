using LiteDB;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Остатки.Classes;


// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace Остатки
{
	/// <summary>
	/// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
	/// </summary>
	/// 
	public sealed partial class remains : Page
	{
		public List<Product> ProductList1 { get; set; }

		public void getRemainsIsBaseThread()
		{
			using (var db = new LiteDatabase($@"{ApplicationData.Current.LocalFolder.Path}/ProductsDB.db"))
			{
				var col = db.GetCollection<Product>("Products");
				List<Product> allProducts = col.Query().OrderBy(x => x.RemainsWhite).ToList();

				ProductList1 = allProducts;

				/*foreach (var item in results)
				{
					if(item.RemainsWhite < 10)
						new ToastContentBuilder()
						.AddArgument("action", "viewConversation")
						.AddArgument("conversationId", 9813)
						.AddText("Осталось меньше 10 товаров!!!")
						.AddText($"Проверь товар: {item.Name} с артикулом: {item.ArticleNumberLerya}")
						.Show();
				} */
			}
		}

		public remains()
		{
			this.InitializeComponent();
			getRemainsIsBaseThread();

			//Thread thread = new Thread(getRemainsIsBaseThread);
			//thread.Start();
		}

		private void GoToWaitRemains_Click(object sender, RoutedEventArgs e)
		{
			var button = sender as Button;
			if (button == null)
				return;

			var item = button.DataContext as Product;
			Message.infoList.Add($"Ждём снова {item.Name}");
			Message.AllErrors();
		}
		private void GoToArchiveRemains_Click(object sender, RoutedEventArgs e)
		{
			var button = sender as Button;
			if (button == null)
				return;

			var item = button.DataContext as Product;
			Message.infoList.Add($"В архив {item.Name}");
			Message.AllErrors();
		}
		private void GoToDelete_Click(object sender, RoutedEventArgs e)
		{
			var button = sender as Button;
			if (button == null)
				return;

			var item = button.DataContext as Product;
			Message.infoList.Add($"Удаляем нафиг {item.Name}");
			Message.AllErrors();
		}
		private void GoToInfo_Click(object sender, RoutedEventArgs e)
		{
			var button = sender as Button;
			if (button == null)
				return;

			var item = button.DataContext as Product;
			Message.infoList.Add($"Показываем инфу {item.Name}");
			Message.AllErrors();
		}
		private void ProductList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			//Message.infoList.Add("НАЖАТО!");
			//Message.AllErrors();
		}

	}
}
