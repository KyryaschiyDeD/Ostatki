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
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Product = Остатки.Classes.Product;
using Остатки.Classes.JobWhithApi.Ozon;
using Остатки.Classes.Petrovich;

namespace Остатки
{
    public class ProductsIdsss
    {
        [JsonProperty("product_id")]
        public List<long> product_id { get; set; }
    }
    public class Susseess
    {
        public bool result { get; set; }
        public int code { get; set; }
        public string message { get; set; }
        public List<object> details { get; set; }
    }

    public sealed partial class remains2 : Page
	{
        static StorageFolder folder = ApplicationData.Current.LocalFolder;

        public ObservableCollection<Product> ProductList1 = new ObservableCollection<Product>();

        private void dg_Sorting(object sender, DataGridColumnEventArgs e)
        {
            //Clear the SortDirection in a previously sorted column when a different column is sorted
            
            switch (e.Column.Tag.ToString())
			{
				case "ArticleNumberInShop":
                    if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                    {
                        dataGridProduct.ItemsSource = new ObservableCollection<Product>(from item in ProductList1
                                                                                        orderby item.ArticleNumberInShop ascending
                                                                                        select item);
                        e.Column.SortDirection = DataGridSortDirection.Ascending;

                    }
                    else
                    {
                        dataGridProduct.ItemsSource = new ObservableCollection<Product>(from item in ProductList1
                                                                                        orderby item.ArticleNumberInShop descending
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
        public void dataGridProduct_CopyingRowClipboardContent(object sender, DataGridRowClipboardEventArgs e)
        {
            e.ClipboardRowContent.Clear();
            e.ClipboardRowContent.Add(new DataGridClipboardCellContent(e.Item, (sender as DataGrid).Columns[0], e.Item.ToString()));
        }
        private void rankLowFilter_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Product> tmpFilterProduct = new ObservableCollection<Product>();
            //tmpFilterProduct = new ObservableCollection<Product>(from item in ProductList1
            //                                                     where item.Name.Contains(FindingTextBox.Text)
            //                                                     select item);
            long myLong;
            bool isNumerical = long.TryParse(FindingTextBox.Text, out myLong);
            if (tmpFilterProduct.Count == 0 && isNumerical)
                tmpFilterProduct = new ObservableCollection<Product>(from item in ProductList1
                                                                     where item.ArticleNumberInShop == myLong.ToString()
                                                                     select item);
            dataGridProduct.ItemsSource = tmpFilterProduct;
        }

        private void Article_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<Product> tmpFilterProduct;
            tmpFilterProduct = new ObservableCollection<Product>(from item in ProductList1
                                                                 where String.IsNullOrEmpty(item.Name)
                                                                 select item);
            dataGridProduct.ItemsSource = tmpFilterProduct;
        }
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(UpdateAllDataBaseLerya);
            thread.Start();
            thread.Join();
        }
        
        private static Susseess PostRequestAsync(ApiKeys key,ProductsIdsss pageOzon)
        {
            var jsort = JsonConvert.SerializeObject(pageOzon);
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-seller.ozon.ru/v1/product/archive");
            httpWebRequest.Headers.Add("Client-Id", key.ClientId);
            httpWebRequest.Headers.Add("Api-Key", key.ApiKey);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var requestStream = httpWebRequest.GetRequestStream())
            using (var writer = new StreamWriter(requestStream))
            {
                writer.Write(jsort);
            }
            HttpWebResponse httpResponse = new HttpWebResponse();
            try
			{
                httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    //ответ от сервера
                    var result = streamReader.ReadToEnd();
                    //Сериализация
                    return JsonConvert.DeserializeObject<Susseess>(result);
                }
            }
			catch (WebException ex)
			{
                HttpWebResponse exeps = (HttpWebResponse)ex.Response;
                if (exeps.StatusCode == HttpStatusCode.BadRequest)
                    return new Susseess() { message = exeps.StatusDescription, result = true };
                else
                    return new Susseess() { message = exeps.StatusDescription, result = false };
            }
            
            
        }
        private void GoArchiveOOO_Click(object sender, RoutedEventArgs e)
        {
            List<Product> allProducts = new List<Product>();
            using (var db = new LiteDatabase($@"{folder.Path}/ProductsDB.db"))
            {
                var col = db.GetCollection<Product>("Products");
                allProducts = col.Query().Where(x => x.RemainsWhite == 0).ToList();
            }
            List<Product> ProductToDell = new List<Product>();
            Dictionary<ApiKeys, List<long>> productAllDict = new Dictionary<ApiKeys, List<long>>();
            Dictionary<Product, int> countOfAPI = new Dictionary<Product, int>();
            using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
            {
                var col = db.GetCollection<Product>("Products");
                allProducts = col.Query().OrderBy(x => x.RemainsWhite).ToList();
            }
            foreach (var keys in ApiKeysesJob.GetAllApiList())
            {
                List<long> product = new List<long>();
                foreach (var item in allProducts)
                {
                    if (product.Count <= 499)
					{
                        if (item.ArticleNumberProductId.ContainsKey(keys.ClientId) && item.ArticleNumberProductId[keys.ClientId].Count != 0)
						{
                            if (item.ArticleNumberProductId[keys.ClientId].First().ArticleOzon != 0 && ((item.RemainsWhite != item.RemainsBlack && item.TypeOfShop == "LeroyMerlen") || item.TypeOfShop == "Леонардо"))
                            {
                                if (item.RemainsWhite == 0 || (item.TypeOfShop == "Леонардо" && item.RemainsWhite <= 1))
                                {
                                    foreach (var valueOzonApi in item.ArticleNumberProductId[keys.ClientId])
                                    {
                                        product.Add(valueOzonApi.ArticleOzon);
                                    }
                                    if (!countOfAPI.ContainsKey(item))
                                        countOfAPI.Add(item, 1);
                                    else
                                        countOfAPI[item]++;
                                }
                            }
                        }
                    }
                   
                }
                productAllDict.Add(keys, product);
            }
			//foreach (var item in allProducts)
			//{
   //             int countAkk = 0;
   //             int countApi = 0;
			//	foreach (var key in ApiKeysesJob.GetAllApiList())
			//	{
   //                 if (item.ArticleNumberProductId.ContainsKey(key.ClientId))
   //                 {
   //                     countAkk++;
   //                 }
   //                 if (countAkk == 2)
			//		{
   //                     if (item.ArticleNumberProductId[key.ClientId].Count == 0)
   //                     {
   //                         countApi++;
   //                     }
   //                 }
   //                 if (countApi == 0)
			//		{
   //                     ProductToDell.Add(item);
   //                 }
   //             }
                
   //         }
            foreach (var item in countOfAPI)
            {
                ProductToDell.Add(item.Key);
            }
            Dictionary<ApiKeys, ProductsIdsss> productsIdsss = new Dictionary<ApiKeys, ProductsIdsss>();

            foreach (var item in productAllDict)
            {
                productsIdsss.Add(item.Key, new ProductsIdsss { product_id = new List<long>(item.Value) });
            }
            int kolvoPost = 0;
            foreach (var item in productsIdsss)
            {
                Message.infoList.Add($"Ключ: {item.Key.ClientId}, {item.Value.product_id.Count}");
            }
            Message.ShowAllToast();
            foreach (var item in productsIdsss)
            {
                Susseess susseess = PostRequestAsync(item.Key, item.Value);
                Message.infoList.Add(susseess.message);
                if (susseess.result)
                {
                    kolvoPost++;
                }
                Message.ShowAllToast();
            }
            if (kolvoPost == productsIdsss.Count())
            {
                foreach (var item in ProductToDell)
                {
                    DataBaseJob.RemainsToWait(item);
                    ProductList1.Remove(item);
                }
                Message.infoList.Add($"Остатки зафиксированы. \n Кол-во: {kolvoPost}");
                Message.ShowAllToast();
            }
        }
        private void GoToGetArhiveByProductId_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private void GetLeroyProducts_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new LiteDatabase($@"{folder.Path}/ProductsDB.db"))
            {
                var col = db.GetCollection<Product>("Products");
                List<Product> allProducts = col.Query().Where(x => x.TypeOfShop == "LeroyMerlen").OrderBy(x => x.RemainsWhite).ToList();
                dataGridProduct.ItemsSource = new ObservableCollection<Product>(allProducts);
                ProductList1 = new ObservableCollection<Product>(allProducts);
            }
        }
        private void GetLeonardoProducts_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new LiteDatabase($@"{folder.Path}/ProductsDB.db"))
            {
                var col = db.GetCollection<Product>("Products");
                List<Product> allProducts = col.Query().Where(x => x.TypeOfShop == "Леонардо").OrderBy(x => x.RemainsWhite).ToList();
                dataGridProduct.ItemsSource = new ObservableCollection<Product>(allProducts);
                ProductList1 = new ObservableCollection<Product>(allProducts);
            }
        }
        private void GetPetrovichProducts_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new LiteDatabase($@"{folder.Path}/ProductsDB.db"))
            {
                var col = db.GetCollection<Product>("Products");
                List<Product> allProducts = col.Query().Where(x => x.TypeOfShop == "petrovich").OrderBy(x => x.RemainsWhite).ToList();
                dataGridProduct.ItemsSource = new ObservableCollection<Product>(allProducts);
                ProductList1 = new ObservableCollection<Product>(allProducts);
            }
        }
        private void GetAllProducts_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new LiteDatabase($@"{folder.Path}/ProductsDB.db"))
            {
                var col = db.GetCollection<Product>("Products");
                List<Product> allProducts = col.Query().OrderBy(x => x.RemainsWhite).ToList();
                dataGridProduct.ItemsSource = new ObservableCollection<Product>(allProducts);
                ProductList1 = new ObservableCollection<Product>(allProducts);
            }
        }
        private void GoToGetProductID_Click(object sender, RoutedEventArgs e)
        {
            CreateToastProductJob();
            GetAndSaveProductId.GetProductsId();
            //GetAndSaveProductId.GetProductsId2();
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
            List<Product> lst = new List<Product>();
            List<ApiKeys> apiKey = ApiKeysesJob.GetAllApiList();
            using (var db = new LiteDatabase($@"{folder.Path}/ProductsDB.db"))
            {
                var col = db.GetCollection<Product>("Products");
                var wait = db.GetCollection<Product>("ProductsWait");
                var archive = db.GetCollection<Product>("ProductsArchive");

                List<Product> Remains = col.Query().ToList();
                List<Product> WaitList = wait.Query().ToList();
                List<Product> ArchiveList = archive.Query().ToList();

                List<Product> allProducts = col.Query().ToList();
                

				

                /*  allProducts.AddRange(wait.Query().ToList());
                  allProducts.AddRange(archive.Query().ToList());

                  foreach (var item in allProducts)
                  {
                      if (item.IsLymarEGOzon || item.IsTheTimeLineOzon)
                      {
                          if (Remains.FindAll(x => x.ProductLink == item.ProductLink).Count >= 1)
                          {
                              if (WaitList.Contains(item))
                                  wait.Delete(item.Id);
                              if (ArchiveList.Contains(item))
                                  archive.Delete(item.Id);
                          }
                          else
                          {
                              if (WaitList.Contains(item))
                                  DataBaseJob.WaitToRemains(item);
                              if (ArchiveList.Contains(item))
                                  DataBaseJob.ArchiveToRemains(item);

                              col = db.GetCollection<Product>("Products");
                              wait = db.GetCollection<Product>("ProductsWait");
                              archive = db.GetCollection<Product>("ProductsArchive");

                              Remains = col.Query().ToList();
                              WaitList = wait.Query().ToList();
                              ArchiveList = archive.Query().ToList();
                          }
                      }
                  }  */
                ProductList1 = new ObservableCollection<Product>(allProducts);
            }
            //DataBaseJob.UpdateList(ProductList1.ToList());
        }
        private void GoOneUpdate_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null)
                return;
            var item = button.DataContext as Product;
            if (item.ProductLink.Contains("leroy"))
                ProductJobs.parseLeryaUpdate(item.ProductLink);
            else
            if (item.ProductLink.Contains("leonardo"))
                ProductJobs.parseLeonardoUpdate(item);
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
        private void GoToInfo_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null)
                return;
            var item = button.DataContext as Product;
            Message.ShowInfoProduct(item.Name, item.ToStringInfo());
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
            List<Product> linksProductLeroy = new List<Product>();
            List<Product> linksProductLeonardo = new List<Product>();
            using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
            {
                var wait = db.GetCollection<Product>("ProductsWait");
                var online = db.GetCollection<Product>("Products");
                //linksProductTXT = online.Query().ToList();
                //linksProductTXT = online.Query().Where(x => x.DateHistoryRemains.Last().Date != DateTime.Now.Date).ToList();
                //linksProductTXT = online.Query().Where(x =>  x.RemainsWhite <= 100 && (x.DateHistoryRemains.Last().Date != DateTime.Now.Date)).ToList();
                //linksProductTXT = online.Query().ToList();

                linksProductLeroy.AddRange(online.Query().Where(x => x.TypeOfShop == "LeroyMerlen").ToList());
                linksProductLeonardo.AddRange(online.Query().Where(x => x.TypeOfShop == "Леонардо").ToList());
                //linksProductLeonardo.AddRange(online.Query().Where(x => x.TypeOfShop == "Леонардо" && x.DateHistoryRemains.Last().Date.Minute <= DateTime.Now.Date.Minute - 5).ToList());
            }
            List<string> allLinksGoToTasksLeroy = new List<string>(linksProductLeroy.ConvertAll(
            new Converter<Product, string>(ProductJobs.GetProductLink)));

            List<string> allLinksGoToTasksLeonardo = new List<string>(linksProductLeonardo.ConvertAll(
            new Converter<Product, string>(ProductJobs.GetProductLink)));

            UpdateProgress(allLinksGoToTasksLeroy.Count() + allLinksGoToTasksLeonardo.Count(), 0,"Плучаем данные");
            ProductJobs.ocherLeroy = new ConcurrentQueue<string>(allLinksGoToTasksLeroy);
            ProductJobs.ocherLeonardo = new ConcurrentQueue<Product>(linksProductLeonardo);
            ProductJobs.productToUpdate = new List<Product>(linksProductLeroy);
            ProductJobs.productToUpdate.AddRange(linksProductLeonardo);

            GoUpdateAllDataBaseLerya();
        }
        public static void GoUpdateAllDataBaseLerya()
		{
            int kolvoToUpdateLeroy = ProductJobs.ocherLeroy.Count;
            int kolvoToUpdateLeonardo = ProductJobs.ocherLeonardo.Count;
            Action action = () =>
            {
                while (!ProductJobs.ocherLeroy.IsEmpty)
                {
                    string str = "";
                    ProductJobs.ocherLeroy.TryDequeue(out str);
                    ProductJobs.parseLeryaUpdate(str);
                    UpdateProgress(kolvoToUpdateLeroy + kolvoToUpdateLeonardo, ProductJobs.NewRemaintProduct.Count(), "Плучаем данные");
                }
            };

            Action action1 = () =>
            {
                while (!ProductJobs.ocherLeonardo.IsEmpty)
                {
                    Product product = new Product();
                    ProductJobs.ocherLeonardo.TryDequeue(out product);
                    ProductJobs.parseLeonardoUpdate(product);
                    UpdateProgress(kolvoToUpdateLeroy + kolvoToUpdateLeonardo, ProductJobs.NewRemaintProduct.Count(), "Плучаем данные");
                }
            };
            Parallel.Invoke(action, action1);
            UpdateProgress(0, 0, "Сохраняем данные");
            DataBaseJob.SaveNewRemains(ProductJobs.NewRemaintProduct);
        }

        public remains2()
        {
            InitializeComponent();
            getRemainsIsBaseThread();
            //Message.errorsList.Add(PetrovichJobsWithCatalog.GetStr());
            Message.AllErrors();
        }

    }

}
