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

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace Остатки
{
	/// <summary>
	/// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
	/// </summary>
	public sealed partial class archiveRemains : Page
	{

        private void dg_Sorting(object sender, DataGridColumnEventArgs e)
        {
            //Clear the SortDirection in a previously sorted column when a different column is sorted

            switch (e.Column.Tag.ToString())
            {
                case "ArticleNumberInShop":
                    if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                    {
                        dataGridProduct.ItemsSource = new ObservableCollection<Product>(from item in ProductListArchive
                                                                                        orderby item.ArticleNumberInShop ascending
                                                                                        select item);
                        e.Column.SortDirection = DataGridSortDirection.Ascending;

                    }
                    else
                    {
                        dataGridProduct.ItemsSource = new ObservableCollection<Product>(from item in ProductListArchive
                                                                                        orderby item.ArticleNumberInShop descending
                                                                                        select item);
                        e.Column.SortDirection = DataGridSortDirection.Descending;
                    }
                    break;
                case "RemainsWhite":
                    if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                    {
                        dataGridProduct.ItemsSource = new ObservableCollection<Product>(from item in ProductListArchive
                                                                                        orderby item.RemainsWhite ascending
                                                                                        select item);
                        e.Column.SortDirection = DataGridSortDirection.Ascending;
                    }
                    else
                    {
                        dataGridProduct.ItemsSource = new ObservableCollection<Product>(from item in ProductListArchive
                                                                                        orderby item.RemainsWhite descending
                                                                                        select item);
                        e.Column.SortDirection = DataGridSortDirection.Descending;
                    }
                    break;
                case "RemainsBlack":
                    if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                    {
                        dataGridProduct.ItemsSource = new ObservableCollection<Product>(from item in ProductListArchive
                                                                                        orderby item.RemainsBlack ascending
                                                                                        select item);
                        e.Column.SortDirection = DataGridSortDirection.Ascending;
                    }
                    else
                    {
                        dataGridProduct.ItemsSource = new ObservableCollection<Product>(from item in ProductListArchive
                                                                                        orderby item.RemainsBlack descending
                                                                                        select item);
                        e.Column.SortDirection = DataGridSortDirection.Descending;
                    }
                    break;
                case "Name":
                    if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                    {
                        dataGridProduct.ItemsSource = new ObservableCollection<Product>(from item in ProductListArchive
                                                                                        orderby item.Name ascending
                                                                                        select item);
                        e.Column.SortDirection = DataGridSortDirection.Ascending;
                    }
                    else
                    {
                        dataGridProduct.ItemsSource = new ObservableCollection<Product>(from item in ProductListArchive
                                                                                        orderby item.Name descending
                                                                                        select item);
                        e.Column.SortDirection = DataGridSortDirection.Descending;
                    }
                    break;
                case "NowPrice":
                    if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                    {
                        dataGridProduct.ItemsSource = new ObservableCollection<Product>(from item in ProductListArchive
                                                                                        orderby item.NowPrice ascending
                                                                                        select item);
                        e.Column.SortDirection = DataGridSortDirection.Ascending;
                    }
                    else
                    {
                        dataGridProduct.ItemsSource = new ObservableCollection<Product>(from item in ProductListArchive
                                                                                        orderby item.NowPrice descending
                                                                                        select item);
                        e.Column.SortDirection = DataGridSortDirection.Descending;
                    }
                    break;
                case "OldPrice":
                    if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                    {
                        dataGridProduct.ItemsSource = new ObservableCollection<Product>(from item in ProductListArchive
                                                                                        orderby item.OldPrice ascending
                                                                                        select item);
                        e.Column.SortDirection = DataGridSortDirection.Ascending;
                    }
                    else
                    {
                        dataGridProduct.ItemsSource = new ObservableCollection<Product>(from item in ProductListArchive
                                                                                        orderby item.OldPrice descending
                                                                                        select item);
                        e.Column.SortDirection = DataGridSortDirection.Descending;
                    }
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
        private void GoToMainRemains_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null)
                return;
            var item = button.DataContext as Product;
            DataBaseJob.ArchiveToRemains(item);
            ProductListArchive.Remove(item);
        }
        private void rankLowFilter_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Product> tmpFilterProduct;
            tmpFilterProduct = new ObservableCollection<Product>(from item in ProductListArchive
                                                                 where item.Name.Contains(FindingTextBox.Text)
                                                                 select item);
            long myLong;
            bool isNumerical = long.TryParse(FindingTextBox.Text, out myLong);
            if (tmpFilterProduct.Count == 0 && isNumerical)
                tmpFilterProduct = new ObservableCollection<Product>(from item in ProductListArchive
                                                                     where item.ArticleNumberInShop == myLong.ToString()
                                                                     select item);

            if (tmpFilterProduct.Count == 0 && isNumerical)
            {
                tmpFilterProduct = new ObservableCollection<Product>(ProductListArchive.Where(x => x.ArticleNumberProductId.Values.SelectMany(y => y).Where(z => z.ArticleOzon == myLong).Count() > 0));
            }

            dataGridProduct.ItemsSource = tmpFilterProduct;
        }

        public ObservableCollection<Product> ProductListArchive = new ObservableCollection<Product>();
        public void getRemainsArchiveIsBaseThread()
        {
            List<Product> allProducts = new List<Product>();
            using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
            {
                var col = db.GetCollection<Product>("ProductsArchive");
                allProducts = col.Query().OrderBy(x => x.RemainsWhite).ToList();
                ProductListArchive = new ObservableCollection<Product>(allProducts);
            }
           /* foreach (var item in allProducts)
            {
                DataBaseJob.ArchiveToRemains(item);
                ProductListArchive.Remove(item);
            }*/
        }

        public archiveRemains()
		{
			this.InitializeComponent();
			getRemainsArchiveIsBaseThread();
		}
	}
}
