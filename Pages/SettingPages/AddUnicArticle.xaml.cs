using LiteDB;
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

namespace Остатки.Pages.SettingPages
{
	/// <summary>
	/// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
	/// </summary>
	public sealed partial class AddUnicArticle : Page
	{
		public static Queue<Product> ProductErrorList = new Queue<Product>();
		public static bool firstStartPage = true;
		public static Product oneProduct = new Product();
		public AddUnicArticle()
		{
			this.InitializeComponent();
			GetProductErrorList();
			GoAnText();
		}
		private void GetProductErrorList()
		{
			ObservableCollection<Product> tmpFilterProduct;
			using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
			{
				var col = db.GetCollection<Product>("Products");
				List<Product> allProducts = col.Query().OrderBy(x => x.RemainsWhite).ToList();
				tmpFilterProduct = new ObservableCollection<Product>(allProducts);
			}
			ProductErrorList = new Queue<Product>(from item in tmpFilterProduct
												  where item.ArticleNumberOzonDict.Count == 0 && String.IsNullOrEmpty(item.ArticleNumberUnic)
													select item);
		}
		private void GoAnText()
		{
			if (ProductErrorList.Count > 0)
			{
				oneProduct = ProductErrorList.Dequeue();
				Names.Text = oneProduct.Name;
				ArticleLeroy.Text = oneProduct.ArticleNumberLerya.ToString();
			}
			else
			{
				Names.Text = "Продуктов с ошибкой артиклов не найдено.";
				GoSaveUnicArticle.Visibility = Visibility.Collapsed;
			}
		}

		private void GoSaveUnicArticle_Click(object sender, RoutedEventArgs e)
		{
			if (oneProduct != null)
			{
				oneProduct.ArticleNumberUnic = ArticleUnicOzon.Text;
			}
			DataBaseJob.UpdateOneProduct(oneProduct);
			GoAnText();
		}
	}
}
