using LiteDB;
using System;
using System.Collections.Generic;
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

namespace Остатки.Pages
{
	/// <summary>
	/// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
	/// </summary>
	public sealed partial class MainInfoList : Page
	{
		public static SatisticGlobal satisticGlobal = new SatisticGlobal();

		private SatisticGlobal GetStatistic()
		{
			List<SatisticGlobal> stat = new List<SatisticGlobal>();
			using (var db = new LiteDatabase($@"{Global.folder.Path}/Globals.db"))
				stat = db.GetCollection<SatisticGlobal>("Statistic").Query().OrderBy(x => x.DateCreate).ToList();
			if (stat.Count > 0)
				return stat.Last();
			else
				return null;
		}
		private void AddTextBlocksinStack00()
		{
			SatisticGlobal satisticGlobal = GetStatistic();
			TextBlock namesBlock = new TextBlock();
			namesBlock.HorizontalAlignment = HorizontalAlignment.Center;
			namesBlock.VerticalAlignment = VerticalAlignment.Center;
			if (satisticGlobal != null)
			{
				namesBlock.Text = "";
				namesBlock.Text += $"Всего: {satisticGlobal.AllProducts} товаров\n";
				namesBlock.Text += $"Остатки: {satisticGlobal.AllProductsRemains} \n";
				namesBlock.Text += $"Ожидаем: {satisticGlobal.AllProductsWait} \n";
				namesBlock.Text += $"Архив: {satisticGlobal.AllProductsArchive } \n";
				namesBlock.Text += $"Удалено: {satisticGlobal.AllProductsDel  } \n\n";

				if (!Double.IsNaN(satisticGlobal.NowAveragePrice) && !Double.IsNaN(satisticGlobal.OldAveragePrice))
                {
					namesBlock.Text += $"Прошлая средняя цена: {satisticGlobal.NowAveragePrice  } \n";
					namesBlock.Text += $"Текущая средняя цена: {satisticGlobal.OldAveragePrice  } \n";
					if (satisticGlobal.NowAveragePrice > satisticGlobal.OldAveragePrice)
						namesBlock.Text += $"Разница в %: {satisticGlobal.OldAveragePrice / satisticGlobal.NowAveragePrice} \n\n";
					else
						namesBlock.Text += $"Разница в %: {satisticGlobal.NowAveragePrice / satisticGlobal.OldAveragePrice * 100} \n\n";
				}

				namesBlock.Text += $"По аккаунтам: \n";
				foreach (var item in satisticGlobal.ProductsAtAccaunt)
				{
					namesBlock.Text += $"\t {item.Key}: {item.Value} \n";
				}
				namesBlock.Text += "\n";
				namesBlock.Text += $"Дата сбора: {satisticGlobal.DateCreate  } \n";
			}
			else
				namesBlock.Text = "Статистики нет :-(";
			Grid00.Children.Clear();
			Grid00.Children.Add(namesBlock);
		}

		private void AddTextBlocksinStack01()
		{
			TextBlock statsBlock = new TextBlock();
			statsBlock.HorizontalAlignment = HorizontalAlignment.Center;
			statsBlock.VerticalAlignment = VerticalAlignment.Center;
			if (satisticGlobal != null)
			{
				statsBlock.Text = "";
				if (satisticGlobal.AllProductsLeroy != 0)
					statsBlock.Text += $"Всего продуктов Леруа: {satisticGlobal.AllProductsLeroy}\n";
				if (satisticGlobal.AllProductsLeroy0 != 0)
					statsBlock.Text += $"\t = 0: \t {satisticGlobal.AllProductsLeroy0} \n";
				if (satisticGlobal.AllProductsLeroy20 != 0)
					statsBlock.Text += $"\t <= 20: \t {satisticGlobal.AllProductsLeroy20} \n";
				if (satisticGlobal.AllProductsLeroy50 != 0)
					statsBlock.Text += $"\t <= 50: \t {satisticGlobal.AllProductsLeroy50} \n";
				if (satisticGlobal.AllProductsLeroy100 != 0)
					statsBlock.Text += $"\t <= 100: \t {satisticGlobal.AllProductsLeroy100} \n";
				if (satisticGlobal.AllProductsLeroy100More != 0)
					statsBlock.Text += $"\t > 100: {satisticGlobal.AllProductsLeroy100More} \n\n";

				if (satisticGlobal.AllProductsLeonardo != 0)
					statsBlock.Text += $"Всего продуктов Леонардо: {satisticGlobal.AllProductsLeonardo} \n";
				if (satisticGlobal.AllProductsLeonardo0 != 0)
					statsBlock.Text += $"\t = 0: \t {satisticGlobal.AllProductsLeonardo0} \n";
				if (satisticGlobal.AllProductsLeonardo20 != 0)
					statsBlock.Text += $"\t <= 20: \t {satisticGlobal.AllProductsLeonardo20} \n";
				if (satisticGlobal.AllProductsLeonardo50 != 0)
					statsBlock.Text += $"\t <= 50: \t {satisticGlobal.AllProductsLeonardo50} \n";
				if (satisticGlobal.AllProductsLeonardo100 != 0)
					statsBlock.Text += $"\t <= 100: \t {satisticGlobal.AllProductsLeonardo100} \n";
				if (satisticGlobal.AllProductsLeonardo100More != 0)
					statsBlock.Text += $"\t > 100: {satisticGlobal.AllProductsLeonardo100More} \n\n";
				//statsBlock.Text += $"Всего позиций Леонардо: \n";

				if (satisticGlobal.AllProductsPetrovich != 0)
					statsBlock.Text += $"Всего продуктов Петрович: {satisticGlobal.AllProductsPetrovich} \n";
				if (satisticGlobal.AllProductsPetrovich0 != 0)
					statsBlock.Text += $"\t = 0: {satisticGlobal.AllProductsPetrovich0} \n";
				if (satisticGlobal.AllProductsPetrovich20 != 0)
					statsBlock.Text += $"\t <= 20: {satisticGlobal.AllProductsPetrovich20} \n";
				if (satisticGlobal.AllProductsPetrovich50 != 0)
					statsBlock.Text += $"\t <= 50: {satisticGlobal.AllProductsPetrovich50} \n";
				if (satisticGlobal.AllProductsPetrovich100 != 0)
					statsBlock.Text += $"\t <= 100: {satisticGlobal.AllProductsPetrovich100} \n";
				if (satisticGlobal.AllProductsPetrovich100More != 0)
					statsBlock.Text += $"\t > 100: {satisticGlobal.AllProductsPetrovich100More} \n\n";

				statsBlock.Text += $"Всего позиций Петрович: \n";
				foreach (var item in satisticGlobal.AllProductsPetrovichPos)
                {
					statsBlock.Text += $"\t{item.Key}: {item.Value}\n";
				}
				statsBlock.Text += $"\n";


				foreach (var item in  satisticGlobal.AllProductsLeonardoPos)
				{
					statsBlock.Text += $"\t {item.Key}: {item.Value} \n";
				}


				statsBlock.Text += $"Всего позиций Леруа: \n";
				foreach (var item in satisticGlobal.AllProductsLeroyPos)
				{
					statsBlock.Text += $"\t {item.Key}: {item.Value} \n";
				}
				statsBlock.Text += $"\n";

				statsBlock.Text += $"Всего проблем с наименованием: {satisticGlobal.ProductProblemName} \n";
				statsBlock.Text += $"Дублирущихся продуктов: {satisticGlobal.DoubleProduct} \n";
			}
			else
				statsBlock.Text = "Статистики нет :-(";
			Grid01.Children.Clear();
			Grid01.Children.Add(statsBlock);
		}

		private void AddTextBlocksinStack10()
		{
			
		}
		private void AddTextBlocksinStack11()
		{
			
		}
		public MainInfoList()
		{
			this.InitializeComponent();
			satisticGlobal  = GetStatistic();
			AddTextBlocksinStack00();
			AddTextBlocksinStack01();
			
		}
		private void UpdateStat_Click(object sender, RoutedEventArgs e)
		{
			SatisticGlobal satisticGl = new SatisticGlobal();
			DataBaseJob.GetAllProductFromTheBase(out List<Product> remainsProduct,out List<Product> waitProduct,out List<Product> archveProduct, out List<Product> delProduct);

			List<Product> AllProducts = new List<Product>();
			AllProducts.AddRange(remainsProduct);
			AllProducts.AddRange(waitProduct);
			AllProducts.AddRange(archveProduct);
			AllProducts.AddRange(delProduct);

			satisticGl.AllProductsRemains = remainsProduct.Count();
			satisticGl.AllProductsWait = waitProduct.Count();
			satisticGl.AllProductsArchive = archveProduct.Count();
			satisticGl.AllProductsDel = delProduct.Count();
			satisticGl.AllProducts = satisticGl.AllProductsRemains + satisticGl.AllProductsWait + satisticGl.AllProductsArchive + satisticGl.AllProductsDel;

			satisticGl.AllProductsLeroy = remainsProduct.Where(x => x.TypeOfShop == "LeroyMerlen").Count();
			satisticGl.AllProductsLeroy0 = remainsProduct.Where(x => x.TypeOfShop == "LeroyMerlen" && x.RemainsWhite == 0).Count();
			satisticGl.AllProductsLeroy20 = remainsProduct.Where(x => x.TypeOfShop == "LeroyMerlen" && x.RemainsWhite <= 20).Count();
			satisticGl.AllProductsLeroy50 = remainsProduct.Where(x => x.TypeOfShop == "LeroyMerlen" && x.RemainsWhite <= 50).Count();
			satisticGl.AllProductsLeroy100 = remainsProduct.Where(x => x.TypeOfShop == "LeroyMerlen" && x.RemainsWhite <= 100).Count();
			satisticGl.AllProductsLeroy100More = remainsProduct.Where(x => x.TypeOfShop == "LeroyMerlen" && x.RemainsWhite > 100).Count();

			satisticGl.AllProductsLeonardo = remainsProduct.Where(x => x.TypeOfShop == "Леонардо").Count();
			satisticGl.AllProductsLeonardo0 = remainsProduct.Where(x => x.TypeOfShop == "Леонардо" && x.RemainsWhite == 0).Count();
			satisticGl.AllProductsLeonardo20 = remainsProduct.Where(x => x.TypeOfShop == "Леонардо" && x.RemainsWhite <= 20).Count();
			satisticGl.AllProductsLeonardo50 = remainsProduct.Where(x => x.TypeOfShop == "Леонардо" && x.RemainsWhite <= 50).Count();
			satisticGl.AllProductsLeonardo100 = remainsProduct.Where(x => x.TypeOfShop == "Леонардо" && x.RemainsWhite <= 100).Count();
			satisticGl.AllProductsLeonardo100More = remainsProduct.Where(x => x.TypeOfShop == "Леонардо" && x.RemainsWhite > 100).Count();

			satisticGl.AllProductsPetrovich = remainsProduct.Where(x => x.TypeOfShop == "petrovich").Count();
			satisticGl.AllProductsPetrovich0 = remainsProduct.Where(x => x.TypeOfShop == "petrovich" && x.RemainsWhite == 0).Count();
			satisticGl.AllProductsPetrovich20 = remainsProduct.Where(x => x.TypeOfShop == "petrovich" && x.RemainsWhite <= 20).Count();
			satisticGl.AllProductsPetrovich50 = remainsProduct.Where(x => x.TypeOfShop == "petrovich" && x.RemainsWhite <= 50).Count();
			satisticGl.AllProductsPetrovich100 = remainsProduct.Where(x => x.TypeOfShop == "petrovich" && x.RemainsWhite <= 100).Count();
			satisticGl.AllProductsPetrovich100More = remainsProduct.Where(x => x.TypeOfShop == "petrovich" && x.RemainsWhite > 100).Count();

			satisticGl.ProductProblemName = 0;
			satisticGl.DoubleProduct = 0;
			List<string> problemLink = new List<string>();

			List<Product> petrovichPos = remainsProduct.Where(x => x.TypeOfShop == "petrovich").ToList();
			bool addKeyes = false;

			satisticGl.AllProductsPetrovichPos = new Dictionary<string, long>();
			foreach (var item in petrovichPos)
			{
				if (String.IsNullOrEmpty(item.Name))
					satisticGl.ProductProblemName++;

				if (!addKeyes)
					foreach (var itemKey in item.ArticleNumberProductId)
					{
						if (!satisticGl.AllProductsPetrovichPos.ContainsKey(itemKey.Key))
							satisticGl.AllProductsPetrovichPos.Add(itemKey.Key, 0);
						if (satisticGl.AllProductsPetrovichPos.Count == ApiKeysesJob.GetAllApiList().Count)
							addKeyes = true;
					}
				foreach (var itemKey in item.ArticleNumberProductId)
				{
					if (itemKey.Key != "181882")
					satisticGl.AllProductsPetrovichPos[itemKey.Key] += itemKey.Value.Count();
				}

			}

			List<Product> leonardoPos = remainsProduct.Where(x => x.TypeOfShop == "Леонардо").ToList();
			addKeyes = false;
			satisticGl.AllProductsLeonardoPos = new Dictionary<string, long>();
			foreach (var item in leonardoPos)
			{
				if (String.IsNullOrEmpty(item.Name))
					satisticGl.ProductProblemName++;

				if (!addKeyes)
				foreach (var itemKey in item.ArticleNumberProductId)
				{
						if (!satisticGl.AllProductsLeonardoPos.ContainsKey(itemKey.Key))
							satisticGl.AllProductsLeonardoPos.Add(itemKey.Key, 0);
						if (satisticGl.AllProductsLeonardoPos.Count == ApiKeysesJob.GetAllApiList().Count)
							addKeyes = true;
				}
				foreach (var itemKey in item.ArticleNumberProductId)
				{

					satisticGl.AllProductsLeonardoPos[itemKey.Key] += itemKey.Value.Count();	
				}
				
			}

			List<Product> leroyPos = remainsProduct.Where(x => x.TypeOfShop == "LeroyMerlen").ToList();
			addKeyes = false;
			satisticGl.AllProductsLeroyPos = new Dictionary<string, long>();
			foreach (var item in leroyPos)
			{
				if (!addKeyes)
					foreach (var itemKey in item.ArticleNumberProductId)
					{
						if (!satisticGl.AllProductsLeroyPos.ContainsKey(itemKey.Key))
							satisticGl.AllProductsLeroyPos.Add(itemKey.Key, 0);
						if (satisticGl.AllProductsLeroyPos.Count == ApiKeysesJob.GetAllApiList().Count)
							addKeyes = true;
					}
				foreach (var itemKey in item.ArticleNumberProductId)
				{
					satisticGl.AllProductsLeroyPos[itemKey.Key] += itemKey.Value.Count();
				}

			}


			Dictionary<string, long> colProductOnApi = new Dictionary<string, long>();

			List<ApiKeys> keys = ApiKeysesJob.GetAllApiList();
			foreach (var item in keys)
			{
				colProductOnApi.Add(item.Name, remainsProduct.Where(x => x.AccauntOzonID.ContainsKey(item.ClientId)).Count());
			}


			int kolvo = remainsProduct.Count;

			foreach (var item in AllProducts)
            {
				if (item.OldPrice.Count > 0)
                {
					satisticGl.NowAveragePrice += item.NowPrice;
					satisticGl.OldAveragePrice += item.OldPriceCh;
				}
				else
                {
					kolvo--;
				}
			}

			satisticGl.NowAveragePrice = satisticGl.NowAveragePrice / kolvo;
			satisticGl.OldAveragePrice = satisticGl.OldAveragePrice / kolvo;

			satisticGl.ProductsAtAccaunt = colProductOnApi;
			satisticGl.DateCreate = DateTime.Now;

			using (var db = new LiteDatabase($@"{Global.folder.Path}/Globals.db"))
			{
				var col = db.GetCollection<SatisticGlobal>("Statistic");
				col.Insert(satisticGl);
			}
			AddTextBlocksinStack00();
			AddTextBlocksinStack01();
		}
	}
}
