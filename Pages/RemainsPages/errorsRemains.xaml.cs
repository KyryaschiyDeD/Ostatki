using LiteDB;
using Microsoft.Toolkit.Uwp.UI.Controls;
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
using Остатки.Classes.ProductsClasses;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace Остатки.Pages.RemainsPages
{
	/// <summary>
	/// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
	/// </summary>
	public sealed partial class errorsRemains : Page
	{
		static DataGrid products = new DataGrid();
		public errorsRemains()
		{
			this.InitializeComponent();
			ObservableCollection<ProductFromMarletplace> productFromMarletplaces = new ObservableCollection<ProductFromMarletplace>();
			products.Name = "DataGridProduct";
			products.IsReadOnly = true;
			products.ItemsSource = productFromMarletplaces;
			products.SelectionMode = DataGridSelectionMode.Single;

			using (var db = new LiteDatabase($@"{Global.folder.Path}/ErrorArticle.db"))
			{
				var col = db.GetCollection<ProductFromMarletplace>("ProductsFromMarletplace");
				List<ProductFromMarletplace> productFromMarletplacesTMP = col.Query().ToList();
				productFromMarletplaces = new ObservableCollection<ProductFromMarletplace>(productFromMarletplacesTMP);
			}
			products.ItemsSource = productFromMarletplaces;
			MainGrid.Children.Add(products);
		}
	}
}
