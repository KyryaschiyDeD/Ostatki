using HtmlAgilityPack;
using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Остатки.Classes;
using Остатки.Classes.JobWhithApi.Analitics.TestAnalitics.Response;
using Остатки.Classes.JobWhithApi.Ozon.Postings;
using Остатки.Classes.Taxation;
using Остатки.Classes.Taxation.TaxClasses;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace Остатки.Pages.Taxation
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class AddTaxJob : Page
    {
        public AddTaxJob()
        {
            this.InitializeComponent();
            /*
             int limit = 1000;
             int offset = 0;

             Classes.JobWhithApi.Ozon.Postings.Request.Filter filter = new Classes.JobWhithApi.Ozon.Postings.Request.Filter();
             filter.since = Convert.ToDateTime("2021-01-01T21:00:00.000Z");
             filter.to = Convert.ToDateTime("2022-01-01T21:00:00.000Z");
             filter.status = "delivered";

             Classes.JobWhithApi.Ozon.Postings.Request.With with = new Classes.JobWhithApi.Ozon.Postings.Request.With();
             with.analytics_data = false;
             with.financial_data = false;

             List<Classes.JobWhithApi.Ozon.Postings.Response.Posting> Postings = new List<Classes.JobWhithApi.Ozon.Postings.Response.Posting>();
             Classes.JobWhithApi.Ozon.Postings.Response.Root PostsRoot = new Classes.JobWhithApi.Ozon.Postings.Response.Root();

             string oneKey = ApiKeysesJob.GetApiByKey("200744");
             do
             {
                 PostsRoot = (Classes.JobWhithApi.Ozon.Postings.Response.Root)PostingSaveJob.GetPostings("200744", oneKey, offset, limit, filter, with, false);

                 if (PostsRoot.result.postings.Count != 0)
                 {
                    foreach (var item in PostsRoot.result.postings)
                    {
                        item.Id = Guid.NewGuid();
                        foreach (var one in item.products)
                        {
                            one.posting_number = item.posting_number;
                            one.shipment_date = item.shipment_date;
                            one.in_process_at = item.in_process_at;
                            one.PostingId = item.Id;
                        }
                    }
                     Postings.AddRange(PostsRoot.result.postings);
                 }
                 offset += PostsRoot.result.postings.Count;
             }
             while (PostsRoot.result.has_next);

             using (var db = new LiteDatabase($@"{Global.folder.Path}/TMPPostingTax.db"))
             {
                 var col = db.GetCollection<Classes.JobWhithApi.Ozon.Postings.Response.Posting>("Postings");
                 col.InsertBulk(Postings);
             }



             
            List<ReceiptOnDB> tmpTaxBD = new List<ReceiptOnDB>();
            using (var db = new LiteDatabase($@"{Global.folder.Path}/TaxsDB.db"))
            {
                var col = db.GetCollection<ReceiptOnDB>("Receipts");
                tmpTaxBD = new List<ReceiptOnDB>(col.Query().ToList());
            }

            foreach (var oneTax in tmpTaxBD)
            {
                foreach (var onePos in oneTax.receipt.items)
                {
                    onePos.remainsQuantity = onePos.quantity;
                }
            }

            using(var db = new LiteDatabase($@"{Global.folder.Path}/TaxsDB.db"))
            {
                var col = db.GetCollection<ReceiptOnDB>("Receipts");
                foreach (var item in tmpTaxBD)
                {
                    col.Update(item);
                }
                
            } 

            List<Classes.JobWhithApi.Ozon.Postings.Response.Posting> Postings = new List<Classes.JobWhithApi.Ozon.Postings.Response.Posting>();

            using (var db = new LiteDatabase($@"{Global.folder.Path}/TMPPostingTax.db"))
            {
                var col = db.GetCollection<Classes.JobWhithApi.Ozon.Postings.Response.Posting>("Postings");
                Postings = col.Query().ToList();
            }

            foreach (var item in Postings)
            {
                foreach (var oneProduct in item.products)
                {
                    oneProduct.in_process_at = item.in_process_at;

                }
            }

            using (var db = new LiteDatabase($@"{Global.folder.Path}/TMPPostingTax.db"))
            {
                var col = db.GetCollection<Classes.JobWhithApi.Ozon.Postings.Response.Posting>("Postings");
                foreach (var item in Postings)
                {
                    col.Update(item);
                }
            }
            */
        }

        private void addTaxCode_Click(object sender, RoutedEventArgs e)
        {
            link.Text.Replace(" ", "");
            if (link.Text.Length != 0)
                TaxationJobClass.GetAndSaveTax(link.Text);
            link.Text = "";
        }

        private async void addTaxTxtWhithCode_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".txt");
            var files = await picker.PickMultipleFilesAsync();
            foreach (var file in files)
            {
                if (file != null)
                {
                    List<string> taxCodeList = new List<string>(await FileIO.ReadLinesAsync(file));
                    foreach (var oneTaxCode in taxCodeList)
                    {
                        if (oneTaxCode.Length != 0)
                        {
                            //Thread.Sleep(750);
                            TaxationJobClass.GetAndSaveTax(oneTaxCode);
                        }
                    }
                };
            }
        }

        private async void addTaxEmlWhithCode_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".eml");
            var files = await picker.PickMultipleFilesAsync();

            List<string> replaceOnQR = new List<string>()
            {
                " ", "\r", "\n", "amp;","am=p;","3D", "\"", "&=", "a=mp;", "amp=;"
            };
            List<string> dataQR = new List<string>();
            foreach (var file in files)
            {
                if (file != null)
                {
                    string taxCodeList = await FileIO.ReadTextAsync(file);
                    if (taxCodeList.Length != 0)
                    {
                        GroupCollection matches = Regex.Match(taxCodeList, @"<img src=3D""https://lk.platformaofd.ru/web/noauth/cheque/qrcode\?(.+?)style",
                            RegexOptions.IgnoreCase | RegexOptions.Singleline).Groups;
                        string str = matches[1].Value;

                        foreach (var oneRep in replaceOnQR)
                        {
                            str = str.Replace(oneRep, "");
                        }
                        str = str.Replace("==", "=").Replace("&=","&");

                        if (!str.Contains("&fp"))
                            str = str.Replace("fp", "&fp");

                        if (str.Length != 0)
                        {
                            //Thread.Sleep(907);
                            //TaxationJobClass.GetAndSaveTax(str);
                            dataQR.Add(str);
                        }
                            
                    }
                    
                };
            }

            int count = dataQR.Count();

            foreach (var item in dataQR)
            {
                Thread.Sleep(907);
                TaxationJobClass.GetAndSaveTax(item);
            }
        }
    }
}
