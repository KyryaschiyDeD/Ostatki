using LiteDB;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Остатки.Classes;
using Остатки.Classes.Taxation;
using Остатки.Classes.Taxation.TaxClasses;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace Остатки.Pages.Taxation
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    /// 
    public class DateTimeToDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (int)value / 100;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (int)value * 100;
        }
    }

    public class TaxToItemsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (int)value / 100;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (int)value * 100;
        }
    }

    public sealed partial class TaxationJob : Page
    {
        public static bool newUp = false;
        //public ObservableCollection<ReceiptOnDB> ProductListArchive = new ObservableCollection<ReceiptOnDB>();
        public ObservableCollection<Classes.Taxation.TaxClasses.Item> ProductListFromAllTaxs = new ObservableCollection<Classes.Taxation.TaxClasses.Item>();
        List<Classes.Taxation.TaxClasses.Item> TmpProductListFromAllTaxs = new List<Classes.Taxation.TaxClasses.Item>();

        public ObservableCollection<Classes.JobWhithApi.Ozon.Postings.Response.Product> ProductListFromOzon = new ObservableCollection<Classes.JobWhithApi.Ozon.Postings.Response.Product>();
        List<Classes.JobWhithApi.Ozon.Postings.Response.Product> TmpProductListFromAllPostings = new List<Classes.JobWhithApi.Ozon.Postings.Response.Product>();
        public ObservableCollection<ReceiptOnDB> tmpTaxBD = new ObservableCollection<ReceiptOnDB>();
        public TaxationJob()
        {
            this.InitializeComponent();


            List<Classes.JobWhithApi.Ozon.Postings.Response.Posting> TmpListPostings = new List<Classes.JobWhithApi.Ozon.Postings.Response.Posting>();

            using (var db = new LiteDatabase($@"{Global.folder.Path}/TaxsDB.db"))
            {
                var col = db.GetCollection<ReceiptOnDB>("Receipts");
                tmpTaxBD = new ObservableCollection<ReceiptOnDB>(col.Query().ToList());
            }
            //Listbbb.ItemsSource = tmpTaxBD;
            using (var db = new LiteDatabase($@"{Global.folder.Path}/TMPPostingTax.db"))
            {
                var col = db.GetCollection<Classes.JobWhithApi.Ozon.Postings.Response.Posting>("Postings");
                TmpListPostings = new List<Classes.JobWhithApi.Ozon.Postings.Response.Posting>(col.Query().ToList());
            }

            int kolTaxWithNotNull = 0;
            List<Classes.Taxation.TaxClasses.Item> TmpProductListFromAllTaxsCount = new List<Classes.Taxation.TaxClasses.Item>();


            foreach (var oneTmpTaxBD in tmpTaxBD)
            {
                if (oneTmpTaxBD.receipt.items.Where(x => !x.IsFound) != null)
                {
                    kolTaxWithNotNull++;
                    TmpProductListFromAllTaxs.AddRange(oneTmpTaxBD.receipt.items.Where(x => !x.IsFound));
                }
                TmpProductListFromAllTaxsCount.AddRange(oneTmpTaxBD.receipt.items);
            }
            foreach (var item in TmpListPostings)
            {
                TmpProductListFromAllPostings.AddRange(item.products.Where(x => !x.Finding));
            }

            /*
             
            foreach (var oneTmpTaxBD in tmpTaxBD)
            {
                TmpProductListFromAllTaxs.AddRange(oneTmpTaxBD.receipt.items.Where(x => x.PostingNumber != null && x.PostingNumber.Count > 0));
            }


            foreach (var item in TmpListPostings)
            {
                TmpProductListFromAllPostings.AddRange(item.products.Where(x => x.Finding));
            }
            */
            int k = 0;
            foreach (var item in TmpProductListFromAllPostings)
            {
                item.ID = ++k;
            }
            ProductListFromOzon = new ObservableCollection<Classes.JobWhithApi.Ozon.Postings.Response.Product>(TmpProductListFromAllPostings);

            ProductListFromAllTaxs = new ObservableCollection<Item>(TmpProductListFromAllTaxs);
            StatOfCountingPost.Text = $"Показано: {ProductListFromOzon.Count()} из {TmpListPostings.SelectMany(x => x.products).Count()}";
            StatOfCountingTax.Text = $"Показано: {kolTaxWithNotNull} из {tmpTaxBD.Count()} чеков  ";
            StatOfCountingTaxItem.Text = $"Показано: {TmpProductListFromAllTaxs.Count()} из {TmpProductListFromAllTaxsCount.Count()} товаров";

        }

        private void rankLowFilter_Click(object sender, RoutedEventArgs e)
        {
            if (FindingTextBox.Text.Length == 0)
                ProductListFromAllTaxs = new ObservableCollection<Item>(TmpProductListFromAllTaxs);
            else
            {
                ProductListFromAllTaxs = new ObservableCollection<Item>(from item in ProductListFromAllTaxs
                                                                        where item.name.ToLower().Contains(FindingTextBox.Text.ToLower())
                                                                        select item);
            }

            dataGridTax.ItemsSource = ProductListFromAllTaxs;
        }
        private void rankLowFilterPost_Click(object sender, RoutedEventArgs e)
        {
            if (FindingTextBoxPost.Text.Length == 0)
                ProductListFromOzon = new ObservableCollection<Classes.JobWhithApi.Ozon.Postings.Response.Product>(TmpProductListFromAllPostings);
            else
            {
                ProductListFromOzon = new ObservableCollection<Classes.JobWhithApi.Ozon.Postings.Response.Product>(from item in ProductListFromOzon
                                                                                                                   where item.name.ToLower().Contains(FindingTextBoxPost.Text.ToLower())
                                                                                                                   select item);
            }

            dataGridPosting.ItemsSource = ProductListFromOzon;
        }


        private void DataShipment_Changed(object sender, DatePickerValueChangedEventArgs e)
        {
            SaveUpdate_Click(new object(), new RoutedEventArgs());

            List<Classes.JobWhithApi.Ozon.Postings.Response.Product> TmpProductListFromAllPostingsCount = new List<Classes.JobWhithApi.Ozon.Postings.Response.Product>();

            List<Classes.JobWhithApi.Ozon.Postings.Response.Posting> TmpListPostings = new List<Classes.JobWhithApi.Ozon.Postings.Response.Posting>();
            TmpProductListFromAllPostings.Clear();
            using (var db = new LiteDatabase($@"{Global.folder.Path}/TMPPostingTax.db"))
            {
                var col = db.GetCollection<Classes.JobWhithApi.Ozon.Postings.Response.Posting>("Postings");
                TmpListPostings = new List<Classes.JobWhithApi.Ozon.Postings.Response.Posting>(col.Query().ToList());
            }

            foreach (var item in TmpListPostings)
            {
                TmpProductListFromAllPostings.AddRange(item.products.Where(x => !x.Finding));
                TmpProductListFromAllPostingsCount.AddRange(item.products);
            }

            ProductListFromOzon = new ObservableCollection<Classes.JobWhithApi.Ozon.Postings.Response.Product>(from item in TmpProductListFromAllPostings
                                                                                                               where item.shipment_date.Date == datePickerShipment.SelectedDate.Value.Date
                                                                                                               && !item.Finding
                                                                                                               select item);
            List<DateTime> dateShippingPost = new List<DateTime>();
            List<DateTime> dateShippingPostAddingFilter = new List<DateTime>();
            int k = 0;
            foreach (var item in ProductListFromOzon)
            {
                item.ID = ++k;
                dateShippingPost.Add(item.in_process_at.Date);
            }


            using (var db = new LiteDatabase($@"{Global.folder.Path}/TaxsDB.db"))
            {
                var col = db.GetCollection<ReceiptOnDB>("Receipts");
                tmpTaxBD = new ObservableCollection<ReceiptOnDB>(col.Query().ToList().Where(x => (x.receipt.dateTime.Date == datePickerShipment.SelectedDate.Value.Date)
                || (x.receipt.dateTime.Date == datePickerShipment.SelectedDate.Value.Date.AddDays(-1))
                || (x.receipt.dateTime.Date == datePickerShipment.SelectedDate.Value.Date.AddDays(-2))));

                dateShippingPostAddingFilter.Add(datePickerShipment.SelectedDate.Value.Date);
                dateShippingPostAddingFilter.Add(datePickerShipment.SelectedDate.Value.Date.AddDays(-1));
                dateShippingPostAddingFilter.Add(datePickerShipment.SelectedDate.Value.Date.AddDays(-2));

                foreach (var OneDateShippingPost in dateShippingPost)
                {
                    if (!dateShippingPostAddingFilter.Contains(OneDateShippingPost))
                    {
                        foreach (var item in col.Query().ToList().Where(x => (x.receipt.dateTime.Date == OneDateShippingPost)))
                        {
                            tmpTaxBD.Add(item);
                        }
                    }
                }
            }

            TmpProductListFromAllTaxs = new List<Item>();
            List<Item> TmpProductListFromAllTaxsCount = new List<Item>();
            foreach (var item in tmpTaxBD)
            {
                TmpProductListFromAllTaxs.AddRange(item.receipt.items.Where(x => !x.IsFound));
                TmpProductListFromAllTaxsCount.AddRange(item.receipt.items);
            }

            ProductListFromAllTaxs = new ObservableCollection<Item>(TmpProductListFromAllTaxs.OrderBy(x => x.quantity));

            dataGridTax.ItemsSource = ProductListFromAllTaxs;
            dataGridPosting.ItemsSource = ProductListFromOzon;

            StatOfCountingPost.Text = $"Показано: {ProductListFromOzon.Count()} из {TmpProductListFromAllPostingsCount.Where(x => x.shipment_date.Date == datePickerShipment.SelectedDate.Value.Date).Count()}";

            StatOfCountingTax.Text = $"Показано: {tmpTaxBD.Count()} чеков  ";
            StatOfCountingTaxItem.Text = $"{TmpProductListFromAllTaxs.Count()} из {TmpProductListFromAllTaxsCount.Count()} позиций";
        }

        private void SaveUpdate_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (newUp)
            {
                List<Item> taxs = ProductListFromAllTaxs.ToList();
                List<Classes.JobWhithApi.Ozon.Postings.Response.Product> products = ProductListFromOzon.ToList();
                foreach (var tax in taxs)
                {
                    if (tax.tmpIDProductFromPosting > 0)
                    {
                        Classes.JobWhithApi.Ozon.Postings.Response.Product product = products.Find(x => x.ID == tax.tmpIDProductFromPosting);

                        int koef = 0;
                        if (product.offer_id.Contains("x10"))
                            koef = 10;
                        else
                        if (product.offer_id.Contains("x5"))
                            koef = 5;
                        else
                        if (product.offer_id.Contains("x3"))
                            koef = 3;
                        else
                        if (product.offer_id.Contains("x2"))
                            koef = 2;
                        else
                            koef = 1;

                        tax.remainsQuantity -= product.quantity * koef;
                        if (tax.remainsQuantity <= 0)
                            tax.IsFound = true;
                        Queue<Classes.JobWhithApi.Ozon.Postings.Response.Product> postingFindingBydy = new Queue<Classes.JobWhithApi.Ozon.Postings.Response.Product>();
                        Queue<Item> texingFindingBydy = new Queue<Item>();
                        using (var dbPost = new LiteDatabase($@"{Global.folder.Path}/TMPPostingTax.db"))
                        {
                            var colPost = dbPost.GetCollection<Classes.JobWhithApi.Ozon.Postings.Response.Posting>("Postings");
                            Classes.JobWhithApi.Ozon.Postings.Response.Posting onePost = colPost.FindById(product.PostingId);
                            Classes.JobWhithApi.Ozon.Postings.Response.Product productUp = onePost.products.Find(x => x.price == product.price && x.sku == product.sku && x.offer_id == product.offer_id && !x.Finding);
                            //.JobWhithApi.Ozon.Postings.Response.Product productUp = onePost.products.Find(x => x.price == product.price && x.sku == product.sku && x.offer_id == product.offer_id);
                            int index = onePost.products.IndexOf(productUp);
                            product.Finding = true;
                            onePost.products[index] = product;
                            colPost.Update(onePost);

                            postingFindingBydy = new Queue<Classes.JobWhithApi.Ozon.Postings.Response.Product>(colPost.Query().ToList().SelectMany(x => x.products).Where(p => p.name == product.name && !p.Finding).OrderBy(z => z.shipment_date).ToList());
                        }
                        if (tax.IdPosting == null)
                            tax.IdPosting = new List<Guid>();
                        if (tax.PostingNumber == null)
                            tax.PostingNumber = new List<string>();
                        tax.IdPosting.Add(product.PostingId);
                        tax.PostingNumber.Add(product.posting_number);
                        using (var dbTax = new LiteDatabase($@"{Global.folder.Path}/TaxsDB.db"))
                        {
                            var coltax = dbTax.GetCollection<ReceiptOnDB>("Receipts");
                            var one = coltax.FindById(tax.ReceiptOnDBID);
                            Item finding = one.receipt.items.Find(x => x.IsFound == false && x.name == tax.name && x.price == tax.price);
                            int index = one.receipt.items.IndexOf(finding);
                            tax.tmpIDProductFromPosting = 0;
                            one.receipt.items[index] = tax;
                            coltax.Update(one);

                            if (coltax.FindById(tax.ReceiptOnDBID).receipt.user.Contains("Планета увлечений"))
                            {
                                Regex rg = new Regex(@"-[\d-]+");
                                MatchCollection matchedTaxName = rg.Matches(tax.name);
                                if (matchedTaxName.Count > 0)
                                {
                                    string findRegTaxName = matchedTaxName.First().ToString();
                                    texingFindingBydy = new Queue<Item>(coltax.Query().ToList()
                                        .SelectMany(x => x.receipt.items)
                                        .Where(z => z.name.ToLower().Contains(findRegTaxName.ToLower()) && !z.IsFound)
                                        .OrderBy(p => p.dateBuy));
                                }

                            }
                            else
                            {
                                texingFindingBydy = new Queue<Item>(coltax.Query().ToList()
                                    .SelectMany(x => x.receipt.items)
                                    .Where(z => z.name.ToLower().Contains(tax.name.ToLower()) && !z.IsFound)
                                    .OrderBy(p => p.dateBuy));
                            }
                        }

                        if (texingFindingBydy.Count() > 0 && postingFindingBydy.Count() > 0)
                            foreach (var onePosOnTax in texingFindingBydy)
                            {
                                if (!onePosOnTax.IsFound)
                                {
                                    Classes.JobWhithApi.Ozon.Postings.Response.Product findingProduct = new Classes.JobWhithApi.Ozon.Postings.Response.Product();
                                    foreach (var oneProductFromPost in postingFindingBydy)
                                    {
                                        if (
                                            oneProductFromPost.shipment_date.Date <= onePosOnTax.dateBuy.Date
                                            && !oneProductFromPost.Finding
                                            && onePosOnTax.remainsQuantity >= oneProductFromPost.quantity
                                            )
                                        {
                                            using (var dbPost = new LiteDatabase($@"{Global.folder.Path}/TMPPostingTax.db"))
                                            {
                                                var colPost = dbPost.GetCollection<Classes.JobWhithApi.Ozon.Postings.Response.Posting>("Postings");

                                                Classes.JobWhithApi.Ozon.Postings.Response.Posting onePost = colPost.FindById(oneProductFromPost.PostingId);
                                                Classes.JobWhithApi.Ozon.Postings.Response.Product productUp = onePost.products.Find(
                                                    x => x.price == oneProductFromPost.price &&
                                                    x.sku == oneProductFromPost.sku &&
                                                    x.offer_id == oneProductFromPost.offer_id &&
                                                    !x.Finding);

                                                oneProductFromPost.Finding = true;
                                                int index = onePost.products.IndexOf(productUp);
                                                onePost.products[index] = oneProductFromPost;
                                                colPost.Update(onePost);
                                            }
                                            if (onePosOnTax.IdPosting == null)
                                                onePosOnTax.IdPosting = new List<Guid>();
                                            if (onePosOnTax.PostingNumber == null)
                                                onePosOnTax.PostingNumber = new List<string>();

                                            onePosOnTax.IdPosting.Add(product.PostingId);
                                            onePosOnTax.PostingNumber.Add(product.posting_number);
                                            onePosOnTax.remainsQuantity -= oneProductFromPost.quantity;
                                            if (onePosOnTax.remainsQuantity == 0)
                                                onePosOnTax.IsFound = true;

                                            using (var dbTax = new LiteDatabase($@"{Global.folder.Path}/TaxsDB.db"))
                                            {
                                                var coltax = dbTax.GetCollection<ReceiptOnDB>("Receipts");

                                                var one = coltax.FindById(onePosOnTax.ReceiptOnDBID);
                                                Item finding = one.receipt.items.Find(
                                                    x => x.IsFound == false &&
                                                    x.name == onePosOnTax.name &&
                                                    x.price == onePosOnTax.price);
                                                int index = one.receipt.items.IndexOf(finding);
                                                one.receipt.items[index] = onePosOnTax;
                                                coltax.Update(one);
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                    }
                }
                SaveUpdate.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                newUp = false;
            }


        }
        private void AddToRasxod_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null)
                return;

            var oneTaxItem = button.DataContext as Classes.Taxation.TaxClasses.Item;
            using (var dbTax = new LiteDatabase($@"{Global.folder.Path}/TaxsDB.db"))
            {
                var coltax = dbTax.GetCollection<ReceiptOnDB>("Receipts");
                var one = coltax.FindById(oneTaxItem.ReceiptOnDBID);
                Item finding = one.receipt.items.Find(x => x.IsFound == false && x.name == oneTaxItem.name && x.price == oneTaxItem.price);
                int index = one.receipt.items.IndexOf(finding);
                oneTaxItem.tmpIDProductFromPosting = 0;
                oneTaxItem.IsFound = true;
                one.receipt.items[index] = oneTaxItem;
                coltax.Update(one);
            }
        }

        private void AddToRasxodYandex_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null)
                return;

            var oneTaxItem = button.DataContext as Classes.Taxation.TaxClasses.Item;
            using (var dbTax = new LiteDatabase($@"{Global.folder.Path}/TaxsDB.db"))
            {
                var coltax = dbTax.GetCollection<ReceiptOnDB>("Receipts");
                var one = coltax.FindById(oneTaxItem.ReceiptOnDBID);
                Item finding = one.receipt.items.Find(x => x.IsFound == false && x.name == oneTaxItem.name && x.price == oneTaxItem.price);
                int index = one.receipt.items.IndexOf(finding);
                oneTaxItem.tmpIDProductFromPosting = 0;
                oneTaxItem.IsFound = true;
                oneTaxItem.comment = "Яндекс";
                one.receipt.items[index] = oneTaxItem;
                coltax.Update(one);
            }
        }
        private void dg_Sorting(object sender, DataGridColumnEventArgs e)
        {
            switch (e.Column.Tag.ToString())
            {
                case "shipment_date":
                    if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                    {
                        dataGridPosting.ItemsSource = new ObservableCollection<Classes.JobWhithApi.Ozon.Postings.Response.Product>(from item in ProductListFromOzon
                                                                                                                                   orderby item.shipment_date ascending
                                                                                                                                   select item);
                        e.Column.SortDirection = DataGridSortDirection.Ascending;

                    }
                    else
                    {
                        dataGridPosting.ItemsSource = new ObservableCollection<Classes.JobWhithApi.Ozon.Postings.Response.Product>(from item in ProductListFromOzon
                                                                                                                                   orderby item.shipment_date descending
                                                                                                                                   select item);
                        e.Column.SortDirection = DataGridSortDirection.Descending;
                    }
                    break;
                case "price":
                    if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                    {
                        dataGridPosting.ItemsSource = new ObservableCollection<Classes.JobWhithApi.Ozon.Postings.Response.Product>(from item in ProductListFromOzon
                                                                                                                                   orderby Convert.ToDouble(item.price.Replace(".", ",")) ascending
                                                                                                                                   select item);
                        e.Column.SortDirection = DataGridSortDirection.Ascending;

                    }
                    else
                    {
                        dataGridPosting.ItemsSource = new ObservableCollection<Classes.JobWhithApi.Ozon.Postings.Response.Product>(from item in ProductListFromOzon
                                                                                                                                   orderby Convert.ToDouble(item.price.Replace(".", ",")) descending
                                                                                                                                   select item);
                        e.Column.SortDirection = DataGridSortDirection.Descending;
                    }
                    break;

                default:
                    break;
            }
            foreach (var dgColumn in dataGridPosting.Columns)
            {
                if (dgColumn != null && e != null)
                    if (dgColumn.Tag.ToString() != e.Column.Tag.ToString())
                    {
                        dgColumn.SortDirection = null;
                    }
            }

        }

        private void dataGridWhiteOrBlackShop_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            SaveUpdate.Visibility = Windows.UI.Xaml.Visibility.Visible;
            newUp = true;
        }

        public void dataGridProduct_CopyingRowClipboardContent(object sender, DataGridRowClipboardEventArgs e)
        {
            e.ClipboardRowContent.Clear();
            e.ClipboardRowContent.Add(new DataGridClipboardCellContent(e.Item, (sender as DataGrid).Columns[0], e.Item.ToString()));
        }
    }
}
