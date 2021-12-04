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
		static SatisticGlobal satisticGlobal = new SatisticGlobal();

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

			TextBlock statsBlock = new TextBlock();
			statsBlock.HorizontalAlignment = HorizontalAlignment.Center;
			statsBlock.VerticalAlignment = VerticalAlignment.Center;
			if (satisticGlobal != null)
			{
				statsBlock.Text = "";
				statsBlock.Text += $"Всего продуктов Леруа: {satisticGlobal.AllProductsLeroy}\n";
				statsBlock.Text += $"\t = 0: {satisticGlobal.AllProductsLeroy0} \n";
				statsBlock.Text += $"\t <= 50: {satisticGlobal.AllProductsLeroy50} \n";
				statsBlock.Text += $"\t <= 100: {satisticGlobal.AllProductsLeroy100} \n";

				statsBlock.Text += $"Всего продуктов Леонардо: {satisticGlobal.AllProductsLeonardo} \n";
				statsBlock.Text += $"\t = 0: {satisticGlobal.AllProductsLeonardo0} \n";
				statsBlock.Text += $"\t <= 50: {satisticGlobal.AllProductsLeonardo50} \n";
				statsBlock.Text += $"\t <= 100: {satisticGlobal.AllProductsLeonardo100} \n";
				statsBlock.Text += $"Всего позиций Леонардо: {satisticGlobal.AllProductsLeonardoPos } \n";
			}
			else
				namesBlock.Text = "Статистики нет :-(";
			Grid01.Children.Clear();
			Grid01.Children.Add(statsBlock);
		}

		private void AddTextBlocksinStack01()
		{
			TextBlock statsBlock = new TextBlock();
			statsBlock.HorizontalAlignment = HorizontalAlignment.Center;
			statsBlock.VerticalAlignment = VerticalAlignment.Center;
			if (satisticGlobal != null)
			{
				statsBlock.Text = "";
				statsBlock.Text += $"Всего продуктов Леруа: {satisticGlobal.AllProductsLeroy}\n";
				statsBlock.Text += $"\t = 0: \t {satisticGlobal.AllProductsLeroy0} \n";
				statsBlock.Text += $"\t <= 50: \t {satisticGlobal.AllProductsLeroy50} \n";
				statsBlock.Text += $"\t <= 100: \t {satisticGlobal.AllProductsLeroy100} \n\n";

				statsBlock.Text += $"Всего продуктов Леонардо: {satisticGlobal.AllProductsLeonardo} \n";
				statsBlock.Text += $"\t = 0: \t {satisticGlobal.AllProductsLeonardo0} \n";
				statsBlock.Text += $"\t <= 50: \t {satisticGlobal.AllProductsLeonardo50} \n";
				statsBlock.Text += $"\t <= 100: \t {satisticGlobal.AllProductsLeonardo100} \n\n";
				statsBlock.Text += $"Всего позиций Леонардо: \n";
				foreach (var item in  satisticGlobal.AllProductsLeonardoPos)
				{
					statsBlock.Text += $"\t {item.Key}: {item.Value} \n";
				}
				statsBlock.Text += $"Всего позиций Леруа: \n";
				foreach (var item in satisticGlobal.AllProductsLeroyPos)
				{
					statsBlock.Text += $"\t {item.Key}: {item.Value} \n";
				}
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
			
			satisticGl.AllProductsRemains = remainsProduct.Count();
			satisticGl.AllProductsWait = waitProduct.Count();
			satisticGl.AllProductsArchive = archveProduct.Count();
			satisticGl.AllProductsDel = delProduct.Count();
			satisticGl.AllProducts = satisticGl.AllProductsRemains + satisticGl.AllProductsWait + satisticGl.AllProductsArchive + satisticGl.AllProductsDel;
			satisticGl.AllProductsLeroy = remainsProduct.Where(x => x.TypeOfShop == "LeroyMerlen").Count();
			satisticGl.AllProductsLeroy0 = remainsProduct.Where(x => x.TypeOfShop == "LeroyMerlen" && x.RemainsWhite == 0).Count();
			satisticGl.AllProductsLeroy50 = remainsProduct.Where(x => x.TypeOfShop == "LeroyMerlen" && x.RemainsWhite <= 50).Count();
			satisticGl.AllProductsLeroy100 = remainsProduct.Where(x => x.TypeOfShop == "LeroyMerlen" && x.RemainsWhite > 50 && x.RemainsWhite <= 100).Count();
			satisticGl.AllProductsLeonardo = remainsProduct.Where(x => x.TypeOfShop == "Леонардо").Count();
			satisticGl.AllProductsLeonardo0 = remainsProduct.Where(x => x.TypeOfShop == "Леонардо" && x.RemainsWhite == 0).Count();
			satisticGl.AllProductsLeonardo50 = remainsProduct.Where(x => x.TypeOfShop == "Леонардо" && x.RemainsWhite <= 50).Count();
			satisticGl.AllProductsLeonardo100 = remainsProduct.Where(x => x.TypeOfShop == "Леонардо" && x.RemainsWhite > 50 && x.RemainsWhite <= 100).Count();

			List<Product> leonardoPos = remainsProduct.Where(x => x.TypeOfShop == "Леонардо").ToList();
			bool addKeyes = false;
			satisticGl.AllProductsLeonardoPos = new Dictionary<string, long>();
			foreach (var item in leonardoPos)
			{
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
					//satisticGl.AllProductsLeonardoPos[itemKey.Key] += item.ArticleNumberProductId[itemKey.Key].Count();

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
					//satisticGl.AllProductsLeonardoPos[itemKey.Key] += item.ArticleNumberProductId[itemKey.Key].Count();

					satisticGl.AllProductsLeroyPos[itemKey.Key] += itemKey.Value.Count();
				}

			}


			Dictionary<string, long> colProductOnApi = new Dictionary<string, long>();

			List<ApiKeys> keys = ApiKeysesJob.GetAllApiList();
			foreach (var item in keys)
			{
				colProductOnApi.Add(item.Name, remainsProduct.Where(x => x.AccauntOzonID.ContainsKey(item.ClientId)).Count());
			}

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
