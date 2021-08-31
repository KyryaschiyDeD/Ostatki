using LiteDB;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Остатки.Classes;

namespace Остатки.Pages
{
	public sealed partial class EditingBalancesAndPrices : Page
	{
		public ObservableCollection<Product> ProductListDataBase = new ObservableCollection<Product>();
		public ObservableCollection<Product> ProductListOzon = new ObservableCollection<Product>();
		public EditingBalancesAndPrices()
		{
			this.InitializeComponent();
			using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
			{
				var col = db.GetCollection<Product>("Products");
				List<Product> allProducts = col.Query().OrderBy(x => x.RemainsWhite).ToList();
				List<Product> allProductsTMP = new List<Product>(from item in allProducts
																				 where (item.OldPrice.Count != 0 && !item.NewPriceIsSave)
																				 orderby item.OldPrice[item.OldPrice.Count - 1] ascending
																				 select item);
				ProductListDataBase = new ObservableCollection<Product>(allProductsTMP);


			}
		}
	}
}
