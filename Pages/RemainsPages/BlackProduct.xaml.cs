using LiteDB;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Остатки.Classes;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace Остатки.Pages.RemainsPages
{
	/// <summary>
	/// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
	/// </summary>
	public sealed partial class BlackProduct : Page
	{
		public ObservableCollection<Product> ProductList = new ObservableCollection<Product>();

		StorageFile file;

		public List<Product> productsBlackToAdd = new List<Product>();

		public List<Product> productsDelFormRemains = new List<Product>();
		public List<Product> productsDelFormArhive = new List<Product>();
		public List<Product> productsDelFormWait = new List<Product>();

		private void GetBlackProducts()
		{
			using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
			{
				var col = db.GetCollection<Product>("ProductsBlackList");
				List<Product> productFromMarletplacesTMP = col.Query().ToList();
				ProductList = new ObservableCollection<Product>(productFromMarletplacesTMP);
			}
			dataGridProduct.ItemsSource = ProductList;
		}

		public BlackProduct()
		{
			this.InitializeComponent();
			GetBlackProducts();
		}

		private async void AddToBlack_Click(object sender, RoutedEventArgs e)
		{
			var picker = new Windows.Storage.Pickers.FileOpenPicker();
			picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
			picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
			picker.FileTypeFilter.Add(".txt");
			file = await picker.PickSingleFileAsync();
			if (file != null)
			{
				List<string> ochered = new List<string>();
				ochered = new List<string>(await FileIO.ReadLinesAsync(file));

				List<Product> allProductsRemains = new List<Product>();
				List<Product> allProductsWait = new List<Product>();
				List<Product> allProductsArhive = new List<Product>();

				using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
				{
					var col = db.GetCollection<Product>("Products");
					allProductsRemains = col.Query().ToList();

					col = db.GetCollection<Product>("ProductsWait");
					allProductsWait = col.Query().ToList();

					col = db.GetCollection<Product>("ProductsArchive");
					allProductsArhive = col.Query().ToList();
				}

				foreach (var oneArticle in ochered)
				{
					string article = oneArticle;
					if (oneArticle.Contains("lnrd"))
						article = article.Replace("lnrd", "ld").Replace("_", "-");

					List<Product> findProduct = new List<Product>();

					findProduct = allProductsRemains.Where(x => x.ArticleNumberUnicList.Contains(article)).ToList();
					if (findProduct.Count != 0)
					{
						foreach (var item in findProduct)
						{
							if (!productsBlackToAdd.Contains(item))
								productsBlackToAdd.Add(item);
							if (!productsDelFormRemains.Contains(item))
								productsDelFormRemains.Add(item);
						}
					}
					else
					{
						findProduct = allProductsWait.Where(x => x.ArticleNumberUnicList.Contains(article)).ToList();
						if (findProduct.Count != 0)
						{
							foreach (var item in findProduct)
							{
								if (!productsBlackToAdd.Contains(item))
									productsBlackToAdd.Add(item);
								if (!productsDelFormWait.Contains(item))
									productsDelFormWait.Add(item);

							}
						}
						else
						{
							findProduct = allProductsArhive.Where(x => x.ArticleNumberUnicList.Contains(article)).ToList();
							if (findProduct.Count != 0)
							{
								foreach (var item in findProduct)
								{
									if (!productsBlackToAdd.Contains(item))
										productsBlackToAdd.Add(item);
									if (!productsDelFormArhive.Contains(item))
										productsDelFormArhive.Add(item);

								}
							}
						}
							
					}
				}
				
				using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
				{

					if (productsDelFormRemains.Count > 0)
					{
						var col = db.GetCollection<Product>("ProductsBlackList");
							col.InsertBulk(productsBlackToAdd);
					}

					if (productsDelFormRemains.Count > 0)
					{
						var col = db.GetCollection<Product>("Products");
						foreach (var item in productsDelFormRemains)
						{
							col.Delete(item.Id);
						}
					}

					if (productsDelFormWait.Count > 0)
					{
						var col = db.GetCollection<Product>("ProductsWait");
						foreach (var item in productsDelFormWait)
						{
							col.Delete(item.Id);
						}
					}

					if (productsDelFormArhive.Count > 0)
					{
						var col = db.GetCollection<Product>("ProductsArchive");
						foreach (var item in productsDelFormArhive)
						{
							col.Delete(item.Id);
						}
					}
				}

			}
		}

		private void DelFromSale_Click(object sender, RoutedEventArgs e)
		{
			
		}
		private void dg_Sorting(object sender, DataGridColumnEventArgs e)
		{
			switch (e.Column.Tag.ToString())
			{
				case "ArticleNumberInShop":
					if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
					{
						dataGridProduct.ItemsSource = new ObservableCollection<Product>(from item in ProductList
																						orderby item.ArticleNumberInShop ascending
																						select item);
						e.Column.SortDirection = DataGridSortDirection.Ascending;

					}
					else
					{
						dataGridProduct.ItemsSource = new ObservableCollection<Product>(from item in ProductList
																						orderby item.ArticleNumberInShop descending
																						select item);
						e.Column.SortDirection = DataGridSortDirection.Descending;
					}
					break;
				case "Name":
					if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
					{
						dataGridProduct.ItemsSource = new ObservableCollection<Product>(from item in ProductList
																						orderby item.Name ascending
																						select item);
						e.Column.SortDirection = DataGridSortDirection.Ascending;
					}
					else
					{
						dataGridProduct.ItemsSource = new ObservableCollection<Product>(from item in ProductList
																						orderby item.Name descending
																						select item);
						e.Column.SortDirection = DataGridSortDirection.Descending;
					}
					break;
				case "NowPrice":
					if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
					{
						dataGridProduct.ItemsSource = new ObservableCollection<Product>(from item in ProductList
																						orderby item.NowPrice ascending
																						select item);
						e.Column.SortDirection = DataGridSortDirection.Ascending;
					}
					else
					{
						dataGridProduct.ItemsSource = new ObservableCollection<Product>(from item in ProductList
																						orderby item.NowPrice descending
																						select item);
						e.Column.SortDirection = DataGridSortDirection.Descending;
					}
					break;
				default:
					break;
			}
			foreach (var dgColumn in dataGridProduct.Columns)
			{
				if (dgColumn != null && e != null)
					if (dgColumn.Tag.ToString() != e.Column.Tag.ToString())
					{
						dgColumn.SortDirection = null;
					}
			}

		}
	}
}
