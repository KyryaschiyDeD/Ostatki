using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Остатки.Classes;
using LiteDB;
using Windows.Storage;
using System.Collections.ObjectModel;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Threading.Tasks;
using MoreLinq;
using System.Collections.Concurrent;
using System.Threading;

namespace Остатки
{
	public sealed partial class remains2 : Page
	{
        static StorageFolder folder = ApplicationData.Current.LocalFolder;

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
                                                                                        where item.OldPrice.Count != 0
                                                                                        orderby item.OldPrice[item.OldPrice.Count-1] ascending
                                                                                        select item);
                        e.Column.SortDirection = DataGridSortDirection.Ascending;
                    }
                    else
                    {
                        dataGridProduct.ItemsSource = new ObservableCollection<Product>(from item in ProductList1
                                                                                        where item.OldPrice.Count != 0
                                                                                        orderby item.OldPrice[item.OldPrice.Count - 1] descending
                                                                                        select item);
                        e.Column.SortDirection = DataGridSortDirection.Descending;
                    }
                    break;
                case "FuncTemplate":
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
        public void dataGridProduct_CopyingRowClipboardContent(object sender, DataGridRowClipboardEventArgs e)
        {
            e.ClipboardRowContent.Clear();
            e.ClipboardRowContent.Add(new DataGridClipboardCellContent(e.Item, (sender as DataGrid).Columns[0], e.Item.ToString()));
        }
        private void rankLowFilter_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Product> tmpFilterProduct;
            tmpFilterProduct = new ObservableCollection<Product>(from item in ProductList1
                                                                 where item.Name.Contains(FindingTextBox.Text)
                                                                 select item);
            long myLong;
            bool isNumerical = long.TryParse(FindingTextBox.Text, out myLong);
            if (tmpFilterProduct.Count == 0 && isNumerical)
                tmpFilterProduct = new ObservableCollection<Product>(from item in ProductList1
                                                                     where item.ArticleNumberLerya == myLong
                                                                     select item);
            dataGridProduct.ItemsSource = tmpFilterProduct;
        }

        private void Article_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Product> tmpFilterProduct;
            tmpFilterProduct = new ObservableCollection<Product>(from item in ProductList1
                                                                 where item.ArticleNumberOzon == 0
                                                                 select item);
            dataGridProduct.ItemsSource = tmpFilterProduct;
        }
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(UpdateAllDataBaseLerya);
            thread.Start();
            thread.Join();
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
            using (var db = new LiteDatabase($@"{folder.Path}/ProductsDB.db"))
            {
                var col = db.GetCollection<Product>("Products");
                List<Product> allProducts = col.Query().OrderBy(x => x.RemainsWhite).ToList();
                ProductList1 = new ObservableCollection<Product>(allProducts);
            }
        }

        private static void GoTasksUpdate(Queue<string> ochered)
		{
            ConcurrentQueue<string> ocher = new ConcurrentQueue<string>(ochered);
            Action action = () =>
            {
                while (!ocher.IsEmpty)
                {
                    string str = "";
                    ocher.TryDequeue(out str);
                    ProductJobs.parseLeryaUpdate(str);
                }
            };
            Parallel.Invoke(action, action, action, action, action, action,
                action, action, action, action, action, action,
                action, action, action, action, action, action
                );
        }

        public void updateAllDataBase()
		{
            List<Product> linksProductTXT = new List<Product>();
            using (var db = new LiteDatabase($@"{folder.Path}/ProductsDB.db"))
            {
                var wait = db.GetCollection<Product>("ProductsWait");
                var online = db.GetCollection<Product>("Products");
                var allList = online.FindAll();
                linksProductTXT = allList.ToList();
                linksProductTXT.AddRange(wait.FindAll());
            }

            List<string> allLinksGoToTasks = new List<string>(linksProductTXT.ConvertAll(
            new Converter<Product, string>(PointFToPoint)));

            var batchedLinks = allLinksGoToTasks.Batch(250);
            Task[] tasks = new Task[batchedLinks.Count()];
            for (int i = 0; i < batchedLinks.Count(); i++)
            {
                Queue<string> ochered = new Queue<string>(batchedLinks.ElementAt(i));
                tasks[i] = Task.Factory.StartNew(() => GoTasksUpdate(ochered));
            }
            Task.WaitAll(tasks);
        }
        public static string PointFToPoint(Product pf)
        {
            return pf.ProductLink;
        }
        
        private void GoOneUpdate_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null)
                return;
            var item = button.DataContext as Product;
            ProductJobs.parseLeryaUpdate(item.ProductLink);
            ProductJobs.UpdateOneProduct();
        }
        private void GoToWaitRemains_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null)
                return;
            var item = button.DataContext as Product;
            DataBaseJob.RemainsToWait(item);
            ProductList1.Remove(item);
        }
        private void GoToArchiveRemains_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null)
                return;
            var item = button.DataContext as Product;
            DataBaseJob.RemainsToArchive(item);
            ProductList1.Remove(item);
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


        private static void CreateToastProductJob()
        {
            string tag = "Update";
            string group = "Lerya";

            // Construct the toast content with data bound fields
            var content = new ToastContentBuilder()
                .AddText("Обновляем!!!")
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
            toast.Data.Values["progressStatus"] = "Работаем...";

            // Provide sequence number to prevent out-of-order updates, or assign 0 to indicate "always update"
            toast.Data.SequenceNumber = 0;

            // Show the toast notification to the user
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
        public static void UpdateProgress(double kolvo, double apply, string status)
        {
            // Construct a NotificationData object;
            string tag = "Update";
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
            data.Values["progressStatus"] = status;

            // Update the existing notification's data by using tag/group
            ToastNotificationManager.CreateToastNotifier().Update(data, tag, group);
        }
        public static void UpdateAllDataBaseLerya()
        {
            CreateToastProductJob();
            List<Product> linksProductTXT = new List<Product>();
            using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
            {
                var wait = db.GetCollection<Product>("ProductsWait");
                var online = db.GetCollection<Product>("Products");
                var allList = online.FindAll();
                linksProductTXT = allList.ToList();
                linksProductTXT.AddRange(wait.FindAll());
            }
            List<string> allLinksGoToTasks = new List<string>(linksProductTXT.ConvertAll(
            new Converter<Product, string>(ProductJobs.GetProductLink)));
            UpdateProgress(allLinksGoToTasks.Count(),0,"Плучаем данные");
            ProductJobs.ocher = new ConcurrentQueue<string>(allLinksGoToTasks);
            Action action = () =>
            {
                while (!ProductJobs.ocher.IsEmpty)
                {
                    string str = "";
                    ProductJobs.ocher.TryDequeue(out str);
                    ProductJobs.parseLeryaUpdate(str);
                    UpdateProgress(allLinksGoToTasks.Count(), ProductJobs.NewRemaintProductLerya.Count(), "Плучаем данные");
                }
            };
            Parallel.Invoke(action, action, action, action, action, action,
                action, action, action, action, action, action,
                action, action, action, action, action, action,
                action, action, action, action, action, action,
                action, action, action, action, action, action,
                action, action, action, action, action, action,
                action, action, action, action, action, action,
                action, action, action, action, action, action,
                action, action, action, action, action, action,
                action, action, action, action, action, action,
                action, action, action, action, action, action,
                action, action, action, action, action, action
                );
            UpdateProgress(0, 0, "Сохраняем данные");
            DataBaseJob.SaveNewRemains(ProductJobs.NewRemaintProductLerya);
        }
        public remains2()
		{
			InitializeComponent();
            //updateAllDataBase();
            //Thread thread = new Thread(updateAllDataBase);
            //thread.Start();
            //thread.Join();

            getRemainsIsBaseThread();
            //PostRequestAsync();
            //Thread thread = new Thread(getRemainsIsBaseThread);
            //thread.Start();
        }

    }

}
