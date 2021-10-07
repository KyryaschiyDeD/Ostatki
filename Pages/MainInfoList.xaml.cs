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
		}
		private void AddTextBlocksinStack01()
		{
			
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
			AddTextBlocksinStack00();
			
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
		}
	}
}
