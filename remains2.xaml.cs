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
using Microsoft.Toolkit.Uwp.UI.Controls;
using Остатки.Classes;
using LiteDB;
using Windows.Storage;
using System.Collections.ObjectModel;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace Остатки
{
    public sealed partial class remains2 : Page
	{
        public ObservableCollection<Product> ProductList1 = new ObservableCollection<Product>();
        private void dg_Sorting(object sender, DataGridColumnEventArgs e)
        {
            //Clear the SortDirection in a previously sorted column when a different column is sorted
            
            switch (e.Column.Tag.ToString())
			{
				case "ArticleNumberLerya":
                    if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                    {
                        dataGridProduct.ItemsSource = new ObservableCollection<Product>(from item in ProductList1
                                                                                        orderby item.ArticleNumberLerya ascending
                                                                                        select item);
                        e.Column.SortDirection = DataGridSortDirection.Ascending;

                    }
                    else
                    {
                        dataGridProduct.ItemsSource = new ObservableCollection<Product>(from item in ProductList1
                                                                                        orderby item.ArticleNumberLerya descending
                                                                                        select item);
                        e.Column.SortDirection = DataGridSortDirection.Descending;
                    }
                    break;
                case "RemainsWhite":
                    if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                    {
                        dataGridProduct.ItemsSource = new ObservableCollection<Product>(from item in ProductList1
                                                                                        orderby item.RemainsWhite ascending
                                                                                        select item);
                        e.Column.SortDirection = DataGridSortDirection.Ascending;
                    }
                    else
                    {
                        dataGridProduct.ItemsSource = new ObservableCollection<Product>(from item in ProductList1
                                                                                        orderby item.RemainsWhite descending
                                                                                        select item);
                        e.Column.SortDirection = DataGridSortDirection.Descending;
                    }
                    break;
                case "RemainsBlack":
                    if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                    {
                        dataGridProduct.ItemsSource = new ObservableCollection<Product>(from item in ProductList1
                                                                                        orderby item.RemainsBlack ascending
                                                                                        select item);
                        e.Column.SortDirection = DataGridSortDirection.Ascending;
                    }
                    else
                    {
                        dataGridProduct.ItemsSource = new ObservableCollection<Product>(from item in ProductList1
                                                                                        orderby item.RemainsBlack descending
                                                                                        select item);
                        e.Column.SortDirection = DataGridSortDirection.Descending;
                    }
                    break;
                case "Name":
                    if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                    {
                        dataGridProduct.ItemsSource = new ObservableCollection<Product>(from item in ProductList1
                                                                                        orderby item.Name ascending
                                                                                        select item);
                        e.Column.SortDirection = DataGridSortDirection.Ascending;
                    }
                    else
                    {
                        dataGridProduct.ItemsSource = new ObservableCollection<Product>(from item in ProductList1
                                                                                        orderby item.Name descending
                                                                                        select item);
                        e.Column.SortDirection = DataGridSortDirection.Descending;
                    }
                    break;
                case "NowPrice":
                    if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                    {
                        dataGridProduct.ItemsSource = new ObservableCollection<Product>(from item in ProductList1
                                                                                        orderby item.NowPrice ascending
                                                                                        select item);
                        e.Column.SortDirection = DataGridSortDirection.Ascending;
                    }
                    else
                    {
                        dataGridProduct.ItemsSource = new ObservableCollection<Product>(from item in ProductList1
                                                                                        orderby item.NowPrice descending
                                                                                        select item);
                        e.Column.SortDirection = DataGridSortDirection.Descending;
                    }
                    break;
                case "OldPrice":
                    if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                    {
                        dataGridProduct.ItemsSource = new ObservableCollection<Product>(from item in ProductList1
                                                                                        orderby item.OldPrice ascending
                                                                                        select item);
                        e.Column.SortDirection = DataGridSortDirection.Ascending;
                    }
                    else
                    {
                        dataGridProduct.ItemsSource = new ObservableCollection<Product>(from item in ProductList1
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

        public void UpdateProgress(double kolvo, double apply)
        {
            // Construct a NotificationData object;
            string tag = "Ostatck";
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

        public void getRemainsIsBaseThread()
        {
            string tag = "Ostatck";
            string group = "Lerya";

            // Construct the toast content with data bound fields
            var content = new ToastContentBuilder()
                .AddText("Загружаем... Много загружаем...")
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
            toast.Data.Values["progressStatus"] = "Грузим...";

            // Provide sequence number to prevent out-of-order updates, or assign 0 to indicate "always update"
            toast.Data.SequenceNumber = 0;

            // Show the toast notification to the user
            ToastNotificationManager.CreateToastNotifier().Show(toast);
            using (var db = new LiteDatabase($@"{ApplicationData.Current.LocalFolder.Path}/ProductsDB.db"))
            {
                var col = db.GetCollection<Product>("Products");
                List<Product> allProducts = col.Query().OrderBy(x => x.RemainsWhite).ToList();
				foreach (var item in allProducts)
				{
                    ProductList1.Add(item);
                    UpdateProgress(Convert.ToDouble(allProducts.Count), Convert.ToDouble(allProducts.Count - (allProducts.Count - ProductList1.Count)));
                }
            }
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
        public remains2()
		{
			InitializeComponent();
            getRemainsIsBaseThread();

        }

    }
}
