using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System;
using System.IO;
using Windows.Storage.Pickers;
using System.Collections.Generic;
using Windows.Storage;
using Остатки.Classes;
using System.Net;
using Newtonsoft.Json;
using System.Threading;
using Остатки.Classes.JobWhithApi.Ozon.Postings.Answer;
using System.Linq;
using Остатки.Classes.JobWhithApi.Ozon.Postings;
using System.Collections.ObjectModel;
using Остатки.Classes.ProductsClasses;
using LiteDB;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace Остатки.Pages.Statistics
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class BestStore : Page
    {
        public List<Filter> statuses = new List<Filter>
        {
            new Filter("awaiting_registration","ожидает регистрации", false ),
            new Filter("acceptance_in_progress","идёт приёмка", false ),
            new Filter("awaiting_approve","ожидает подтверждения", false ),
            new Filter("awaiting_packaging","ожидает упаковки", true ),
            new Filter("awaiting_deliver","ожидает отгрузки", true ),
            new Filter("arbitration","арбитраж", false ),
            new Filter("client_arbitration","клиентский арбитраж доставки", false ),
            new Filter("delivering","доставляется", true ),
            new Filter("driver_pickup","у водителя", false ),
            new Filter("delivered","доставлено", true ),
            new Filter("cancelled","отменено", true ),
            new Filter("not_accepted","не принят на сортировочном центре", false )
        };

        public BestStore()
        {
            this.InitializeComponent();
        }

        private async void TestExcel_Click(object sender, RoutedEventArgs e)
        {
            List<ApiKeys> apiKeys = ApiKeysesJob.GetAllApiList();
            Dictionary<ApiKeys, List<long>> productAllDict = new Dictionary<ApiKeys, List<long>>();

            foreach (var oneKey in apiKeys)
            {
                Classes.JobWhithApi.Ozon.Postings.Response.Root PostsRoot = new Classes.JobWhithApi.Ozon.Postings.Response.Root();
                if (oneKey.ItIsTop)
                {

                    int petrFBS = 0;
                    int petrFBO = 0;
                    int LeroyFBO = 0;
                    int LeroyFBS = 0;

                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    var package = new ExcelPackage();
                    List<Posting> AllPostings = new List<Posting>();
                    foreach (var status in statuses)
                    {
                        int limit = 1000;
                        int offset = 0;

                        Classes.JobWhithApi.Ozon.Postings.Request.Filter filter = new Classes.JobWhithApi.Ozon.Postings.Request.Filter();
                        filter.since = Convert.ToDateTime("2022-04-21T21:00:00.000Z");
                        filter.to = Convert.ToDateTime("2022-07-23T21:00:00.000Z");
                        filter.status = status.EngName;

                        Classes.JobWhithApi.Ozon.Postings.Request.With with = new Classes.JobWhithApi.Ozon.Postings.Request.With();
                        with.analytics_data = false;
                        with.financial_data = false;

                        List<Classes.JobWhithApi.Ozon.Postings.Response.Product> products = new List<Classes.JobWhithApi.Ozon.Postings.Response.Product>();
                        List<Classes.JobWhithApi.Ozon.Postings.ResponseFBO.Product> productsFBO = new List<Classes.JobWhithApi.Ozon.Postings.ResponseFBO.Product>();
                        List<Classes.JobWhithApi.Ozon.Postings.ResponseFBO.Root> PostsRootFBOList = new List<Classes.JobWhithApi.Ozon.Postings.ResponseFBO.Root>();

                        do
                        {
                            PostsRoot = (Classes.JobWhithApi.Ozon.Postings.Response.Root)PostingSaveJob.GetPostings(oneKey.ClientId, oneKey.ApiKey, offset, limit, filter, with, false);
                                
                            if (PostsRoot.result.postings.Count != 0)
                            {
                                foreach (var onePosting in PostsRoot.result.postings)
                                {
                                    products.AddRange(onePosting.products);
                                }
                            }
                            offset += PostsRoot.result.postings.Count;
                        }
                        while (PostsRoot.result.has_next);

                        if (status.IsFbo)
                        {
                            filter.since = Convert.ToDateTime("2022-04-21T21:00:00.000Z");
                            filter.to = Convert.ToDateTime("2022-05-23T21:00:00.000Z");
                            PostsRootFBOList.Add((Classes.JobWhithApi.Ozon.Postings.ResponseFBO.Root)PostingSaveJob.GetPostings(oneKey.ClientId, oneKey.ApiKey, 0, 1000, filter, with, true));

                            filter.since = Convert.ToDateTime("2022-05-23T21:00:00.000Z");
                            filter.to = Convert.ToDateTime("2022-06-23T21:00:00.000Z");
                            PostsRootFBOList.Add((Classes.JobWhithApi.Ozon.Postings.ResponseFBO.Root)PostingSaveJob.GetPostings(oneKey.ClientId, oneKey.ApiKey, 0, 1000, filter, with, true));

                            filter.since = Convert.ToDateTime("2022-06-23T21:00:00.000Z");
                            filter.to = Convert.ToDateTime("2022-07-23T21:00:00.000Z");
                            PostsRootFBOList.Add((Classes.JobWhithApi.Ozon.Postings.ResponseFBO.Root)PostingSaveJob.GetPostings(oneKey.ClientId, oneKey.ApiKey, 0, 1000, filter, with, true));

                            foreach (var PostsRootFBO in PostsRootFBOList)
                            {
                                if (PostsRootFBO.result.Count != 0)
                                {
                                    foreach (var onePosting in PostsRootFBO.result)
                                    {
                                        productsFBO.AddRange(onePosting.products);
                                    }
                                }
                            }
                        }

                       

                        List<Posting> postings = new List<Posting>();

                        foreach (var oneProduct in products)
                        {
                            var findingPost = postings.FindIndex(x => x.offer_id == oneProduct.offer_id);
                            if (findingPost != -1)
                            {
                                postings[findingPost].quantity += oneProduct.quantity;
                            }
                            else
                            {
                                postings.Add(new Posting() { name = oneProduct.name, offer_id = oneProduct.offer_id, quantity = oneProduct.quantity, sku = oneProduct.sku });
                            }
                        }

                        foreach (var oneProduct in productsFBO)
                        {
                            var findingPost = postings.FindIndex(x => x.offer_id == oneProduct.offer_id);
                            if (findingPost != -1)
                            {
                                postings[findingPost].FBOQuantity += oneProduct.quantity;
                                postings[findingPost].itIsFBO = true;
                            }
                            else
                            {
                                postings.Add(new Posting() { name = oneProduct.name, offer_id = oneProduct.offer_id, sku = oneProduct.sku, itIsFBO = true, FBOQuantity = oneProduct.quantity });
                            }
                        }

                        AllPostings.AddRange(postings);

                        var sortPostings = postings.OrderByDescending(x => x.quantity).ToList();

                        if (sortPostings.Count != 0)
                        {
                            var sheet = package.Workbook.Worksheets
                            .Add(status.RuName);

                            sheet.Cells[1, 1].Value = "Леруа:";
                            sheet.Cells[2, 1].Value = "FBS:";

                            sheet.Cells[3, 1].Value = "Артикул:";
                            sheet.Cells[3, 2].Value = "sku:";
                            sheet.Cells[3, 3].Value = "Наименование:";
                            sheet.Cells[3, 4].Value = "Кол-во:";
                            
                            sheet.Cells[2, 6].Value = "FBO:";

                            sheet.Cells[3, 6].Value = "Артикул:";
                            sheet.Cells[3, 7].Value = "sku:";
                            sheet.Cells[3, 8].Value = "Наименование:";
                            sheet.Cells[3, 9].Value = "По FBS:";
                            sheet.Cells[3, 10].Value = "По FB0:";


                            sheet.Cells[1, 12].Value = "Петя:";
                            sheet.Cells[2, 12].Value = "FBS:";

                            sheet.Cells[3, 12].Value = "Артикул:";
                            sheet.Cells[3, 13].Value = "sku:";
                            sheet.Cells[3, 14].Value = "Наименование:";
                            sheet.Cells[3, 15].Value = "Кол-во:";

                            sheet.Cells[2, 17].Value = "FBO:";

                            sheet.Cells[3, 17].Value = "Артикул:";
                            sheet.Cells[3, 18].Value = "sku:";
                            sheet.Cells[3, 19].Value = "Наименование:";
                            sheet.Cells[3, 20].Value = "По FBS:";
                            sheet.Cells[3, 21].Value = "По FB0:";

                            petrFBS = 0;
                            petrFBO = 0;
                            LeroyFBO = 0;
                            LeroyFBS = 0;

                            for (int i = 0; i < sortPostings.Count; i++)
                            {
                                if(!sortPostings[i].offer_id.Contains("pv"))
                                {
                                    if (sortPostings[i].itIsFBO)
                                    {
                                        sheet.Cells[LeroyFBO + 4, 6].Value = sortPostings[i].offer_id;
                                        sheet.Cells[LeroyFBO + 4, 7].Value = sortPostings[i].sku;
                                        sheet.Cells[LeroyFBO + 4, 8].Value = sortPostings[i].name;
                                        sheet.Cells[LeroyFBO + 4, 9].Value = sortPostings[i].quantity;
                                        sheet.Cells[LeroyFBO + 4, 10].Value = sortPostings[i].FBOQuantity;
                                        LeroyFBO++;
                                    }
                                    else
                                    {
                                        sheet.Cells[LeroyFBS + 4, 1].Value = sortPostings[i].offer_id;
                                        sheet.Cells[LeroyFBS + 4, 2].Value = sortPostings[i].sku;
                                        sheet.Cells[LeroyFBS + 4, 3].Value = sortPostings[i].name;
                                        sheet.Cells[LeroyFBS + 4, 4].Value = sortPostings[i].quantity;
                                        LeroyFBS++;
                                    }
                                }
                                else
                                {
                                    if (sortPostings[i].itIsFBO)
                                    {
                                        sheet.Cells[petrFBO + 4, 17].Value = sortPostings[i].offer_id;
                                        sheet.Cells[petrFBO + 4, 18].Value = sortPostings[i].sku;
                                        sheet.Cells[petrFBO + 4, 19].Value = sortPostings[i].name;
                                        sheet.Cells[petrFBO + 4, 20].Value = sortPostings[i].quantity;
                                        sheet.Cells[petrFBO + 4, 21].Value = sortPostings[i].FBOQuantity;
                                        petrFBO++;
                                    }
                                    else
                                    {
                                        sheet.Cells[petrFBS + 4, 12].Value = sortPostings[i].offer_id;
                                        sheet.Cells[petrFBS + 4, 13].Value = sortPostings[i].sku;
                                        sheet.Cells[petrFBS + 4, 14].Value = sortPostings[i].name;
                                        sheet.Cells[petrFBS + 4, 15].Value = sortPostings[i].quantity;
                                        petrFBS++;
                                    }
                                }
                            }
                        }
                    }

                    int kolVo = AllPostings.Count;

                    List<Posting> AllPostingsNoDup = new List<Posting>();

                    int countWhithDup = AllPostings.Count;

                    for (int i = 0; i < countWhithDup; i++)
                    {
                        var findDup = AllPostings.FindAll(x => x.sku == AllPostings[i].sku);
                        Posting newSumPost = new Posting() { name = findDup.First().name, offer_id = findDup.First().offer_id, sku = findDup.First().sku, quantity = 0 };
                        foreach (var oneDup in findDup)
                        {
                            newSumPost.quantity += oneDup.quantity;
                            //AllPostings.Remove(oneDup);
                            countWhithDup--;
                        }
                        AllPostingsNoDup.Add(newSumPost);
                    }


                    var sortPostingsAll = AllPostings.OrderByDescending(x => x.quantity).ToList();
                   // var sortPostingsAll = AllPostingsNoDup.OrderByDescending(x => x.quantity).ToList();

                    var sheetAll = package.Workbook.Worksheets
                            .Add("Сумма");

                    sheetAll.Cells[1, 1].Value = "Леруа:";
                    sheetAll.Cells[2, 1].Value = "FBS:";

                    sheetAll.Cells[3, 1].Value = "Артикул:";
                    sheetAll.Cells[3, 2].Value = "sku:";
                    sheetAll.Cells[3, 3].Value = "Наименование:";
                    sheetAll.Cells[3, 4].Value = "Кол-во:";

                    sheetAll.Cells[2, 6].Value = "FBO:";

                    sheetAll.Cells[3, 6].Value = "Артикул:";
                    sheetAll.Cells[3, 7].Value = "sku:";
                    sheetAll.Cells[3, 8].Value = "Наименование:";
                    sheetAll.Cells[3, 9].Value = "По FBS:";
                    sheetAll.Cells[3, 10].Value = "По FB0:";


                    sheetAll.Cells[1, 12].Value = "Петя:";
                    sheetAll.Cells[2, 12].Value = "FBS:";

                    sheetAll.Cells[3, 12].Value = "Артикул:";
                    sheetAll.Cells[3, 13].Value = "sku:";
                    sheetAll.Cells[3, 14].Value = "Наименование:";
                    sheetAll.Cells[3, 15].Value = "Кол-во:";

                    sheetAll.Cells[2, 17].Value = "FBO:";

                    sheetAll.Cells[3, 17].Value = "Артикул:";
                    sheetAll.Cells[3, 18].Value = "sku:";
                    sheetAll.Cells[3, 19].Value = "Наименование:";
                    sheetAll.Cells[3, 20].Value = "По FBS:";
                    sheetAll.Cells[3, 21].Value = "По FB0:";

                    petrFBS = 0;
                    petrFBO = 0;
                    LeroyFBO = 0;
                    LeroyFBS = 0;
                    for (int i = 0; i < sortPostingsAll.Count; i++)
                    {
                        if (!sortPostingsAll[i].offer_id.Contains("pv"))
                        {
                            if (sortPostingsAll[i].itIsFBO)
                            {
                                sheetAll.Cells[LeroyFBO + 4, 6].Value = sortPostingsAll[i].offer_id;
                                sheetAll.Cells[LeroyFBO + 4, 7].Value = sortPostingsAll[i].sku;
                                sheetAll.Cells[LeroyFBO + 4, 8].Value = sortPostingsAll[i].name;
                                sheetAll.Cells[LeroyFBO + 4, 9].Value = sortPostingsAll[i].quantity;
                                sheetAll.Cells[LeroyFBO + 4, 10].Value = sortPostingsAll[i].FBOQuantity;
                                LeroyFBO++;
                            }
                            else
                            {
                                sheetAll.Cells[LeroyFBS + 4, 1].Value = sortPostingsAll[i].offer_id;
                                sheetAll.Cells[LeroyFBS + 4, 2].Value = sortPostingsAll[i].sku;
                                sheetAll.Cells[LeroyFBS + 4, 3].Value = sortPostingsAll[i].name;
                                sheetAll.Cells[LeroyFBS + 4, 4].Value = sortPostingsAll[i].quantity;
                                LeroyFBS++;
                            }
                        }
                        else
                        {
                            if (sortPostingsAll[i].itIsFBO)
                            {
                                sheetAll.Cells[petrFBO + 4, 17].Value = sortPostingsAll[i].offer_id;
                                sheetAll.Cells[petrFBO + 4, 18].Value = sortPostingsAll[i].sku;
                                sheetAll.Cells[petrFBO + 4, 19].Value = sortPostingsAll[i].name;
                                sheetAll.Cells[petrFBO + 4, 20].Value = sortPostingsAll[i].quantity;
                                sheetAll.Cells[petrFBO + 4, 21].Value = sortPostingsAll[i].FBOQuantity;
                                petrFBO++;
                            }
                            else
                            {
                                sheetAll.Cells[petrFBS + 4, 12].Value = sortPostingsAll[i].offer_id;
                                sheetAll.Cells[petrFBS + 4, 13].Value = sortPostingsAll[i].sku;
                                sheetAll.Cells[petrFBS + 4, 14].Value = sortPostingsAll[i].name;
                                sheetAll.Cells[petrFBS + 4, 15].Value = sortPostingsAll[i].quantity;
                                petrFBS++;
                            }
                        }
                    }

                    Dictionary<ApiKeys, ProductsIdsss> productsIdsss = new Dictionary<ApiKeys, ProductsIdsss>();

                    List<Posting> longsPost = new List<Posting>();

                    foreach (var item in sortPostingsAll)
                    {
                        if(item.quantity >= 2)
                            longsPost.Add(item);
                    }

                    ObservableCollection<ProductFromMarletplace> productFromMarletplaces = new ObservableCollection<ProductFromMarletplace>();

                    using (var db = new LiteDatabase($@"{Global.folder.Path}/ArticlePRoductFromMarket.db"))
                    {
                        var col = db.GetCollection<ProductFromMarletplace>("ProductsFromMarletplace");
                        List<ProductFromMarletplace> productFromMarletplacesTMP = col.Query().ToList();
                        productFromMarletplaces = new ObservableCollection<ProductFromMarletplace>(productFromMarletplacesTMP);
                    }

                    var savePicker = new FileSavePicker();
                    // место для сохранения по умолчанию
                    savePicker.SuggestedStartLocation = PickerLocationId.Downloads;
                    // устанавливаем типы файлов для сохранения
                    savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".xlsx" });
                    // устанавливаем имя нового файла по умолчанию
                    savePicker.SuggestedFileName = oneKey.ClientId;
                    savePicker.CommitButtonText = "Сохранить";

                    var new_file = await savePicker.PickSaveFileAsync();
                    if (new_file != null)
                    {
                        await FileIO.WriteBytesAsync(new_file, package.GetAsByteArray());
                    }
                }

               
            }

        }

        
    }

}
