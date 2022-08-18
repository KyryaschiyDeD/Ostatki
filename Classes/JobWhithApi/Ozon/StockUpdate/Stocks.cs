using LiteDB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Остатки.Classes.ProductsClasses;

namespace Остатки.Classes.JobWhithApi.Ozon.StockUpdate
{

    public class Stock
    {
        [JsonProperty("offer_id")]
        public string offer_id { get; set; }
        [JsonProperty("product_id")]
        public int product_id { get; set; }
        [JsonProperty("stock")]
        public int stock { get; set; }
    }

    public class Root
    {
        [JsonProperty("stocks")]
        public List<Stock> stocks { get; set; }
    }

    public class Stocks
	{
        public async static void GoUpdateStocks(List<Product> products)
        {
            List<Stock> stocks = new List<Stock>();
            int countToUpdateStock = 0;
            List<RootResp> resp = new List<RootResp>();
            foreach (var keys in ApiKeysesJob.GetAllApiList())
            {
                /*List<ProductFromMarletplace> products1 = new List<ProductFromMarletplace>(); 
                using (var db = new LiteDatabase($@"{Global.folder.Path}/ArticlePRoductFromMarket.db"))
                {
                    var col = db.GetCollection<ProductFromMarletplace>("ProductsFromMarletplace");
                    List<ProductFromMarletplace> productFromMarletplacesTMP = col.Query().ToList();
                    products1 = new List<ProductFromMarletplace>(productFromMarletplacesTMP);
                }
                foreach (var product in products1)
                {
                    Stock stock = new Stock();
                    stock.stock = 0;
                    stock.product_id = (int)product.product_id;
                    stock.offer_id = product.offer_id;

                    stocks.Add(stock);
                    countToUpdateStock++;

                    if (countToUpdateStock >= 450)
                    {
                        countToUpdateStock = 0;
                        SendReqwest(keys.ClientId, keys.ApiKey, stocks);
                        stocks = new List<Stock>();
                    }
                }*/
                foreach (var product in products)
                {
                    if (product.RemainsWhite >= 5)
                    {
                        if (product.ArticleNumberProductId.ContainsKey(keys.ClientId) && product.ArticleNumberProductId[keys.ClientId].Count != 0)
                        {
                            foreach (var articleNumber in product.ArticleNumberProductId[keys.ClientId])
                            {
                                Stock stock = new Stock();
                                stock.stock = 25;
                                stock.product_id = (int)articleNumber.ArticleOzon;
                                stock.offer_id = articleNumber.OurArticle;

                                stocks.Add(stock);
                                countToUpdateStock++;

                                if (countToUpdateStock >= 450)
                                {
                                    countToUpdateStock = 0;
                                    if (!keys.ClientId.Contains("181882"))
                                    {
                                        resp.Add(SendReqwest(keys.ClientId, keys.ApiKey, stocks));
                                    }

                                    stocks = new List<Stock>();
                                }
                            }

                        }
                    }
                }
                countToUpdateStock = 0;
                stocks = new List<Stock>();
                string str = "";
                int kolErrors = 0;

                    resp.Add(SendReqwest(keys.ClientId, keys.ApiKey, stocks));
                    
                    foreach (var result in resp)
                    {
                        foreach (var oneResult in result.result)
                        {
                            if (!oneResult.updated)
                            {
                                kolErrors++;
                                str += oneResult.offer_id + " ";
                                foreach (var item in oneResult.errors)
                                {
                                    str += item.ToString() + " ";
                                }

                                str += "\n";
                                str += "------------------------------------" + "\n";
                            }
                        }
                    }


                   
                if (kolErrors > 0)
                {
                    FolderPicker folderPicker = new FolderPicker();
                    folderPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                    folderPicker.FileTypeFilter.Add("*");
                    StorageFolder fileWithLinks = await folderPicker.PickSingleFolderAsync();
                    await fileWithLinks.CreateFileAsync($"Ошибка обновления остатков {keys.ClientId}.txt", CreationCollisionOption.ReplaceExisting);
                    StorageFile myFile = await fileWithLinks.GetFileAsync($"Ошибка обновления остатков {keys.ClientId}.txt");
                    await FileIO.WriteTextAsync(myFile, str);
                }
            }

           
        }

        private static RootResp SendReqwest(string clientId, string apiKey, List<Stock> stocks)
        {
           
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-seller.ozon.ru/v1/product/import/stocks");
                httpWebRequest.Headers.Add("Client-Id", clientId);
                httpWebRequest.Headers.Add("Api-Key", apiKey);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                Root rootRequest = new Root();
                rootRequest.stocks = stocks;


                var jsort = JsonConvert.SerializeObject(rootRequest);
                using (var requestStream = httpWebRequest.GetRequestStream())
                using (var writer = new StreamWriter(requestStream))
                {
                    writer.Write(jsort);
                }
                Thread.Sleep(500);
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    //ответ от сервера
                    var result = streamReader.ReadToEnd();

                    //Сериализация
                    return JsonConvert.DeserializeObject<RootResp>(result);
                }

        }


        public async static void GoUpdateStocksToNull(List<ArticleNumber> products, ApiKeys keys)
        {
            List<Stock> stocks = new List<Stock>();
            int countToUpdateStock = 0;
            List<RootResp> resp = new List<RootResp>();

            foreach (var product in products)
            {
                Stock stock = new Stock();
                stock.stock = 0;
                stock.product_id = (int)product.ArticleOzon;
                stock.offer_id = product.OurArticle;

                stocks.Add(stock);
                countToUpdateStock++;

                if (countToUpdateStock >= 450)
                {
                    countToUpdateStock = 0;
                    resp.Add(SendReqwest(keys.ClientId, keys.ApiKey, stocks));
                    stocks = new List<Stock>();
                }
            }

            countToUpdateStock = 0;
            resp.Add(SendReqwest(keys.ClientId, keys.ApiKey, stocks));
            stocks = new List<Stock>();
            string str = "";
            int kolErrors = 0;
            foreach (var result in resp)
            {
                foreach (var oneResult in result.result)
                {
                    if (!oneResult.updated)
                    {
                        kolErrors++;
                        str += oneResult.offer_id + " ";
                        foreach (var item in oneResult.errors)
                        {
                            str += item.ToString() + " ";
                        }

                        str += "\n";
                        str += "------------------------------------" + "\n";
                    }
                }
            }
            if (kolErrors > 0)
            {
                FolderPicker folderPicker = new FolderPicker();
                folderPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                folderPicker.FileTypeFilter.Add("*");
                StorageFolder fileWithLinks = await folderPicker.PickSingleFolderAsync();
                await fileWithLinks.CreateFileAsync($"Ошибка обновления остатков {keys.ClientId}.txt", CreationCollisionOption.ReplaceExisting);
                StorageFile myFile = await fileWithLinks.GetFileAsync($"Ошибка обновления остатков {keys.ClientId}.txt");
                await FileIO.WriteTextAsync(myFile, str);
            }

        }
    }


    public class Result
    {
        [JsonProperty("product_id")]
        public int product_id { get; set; }
        [JsonProperty("offer_id")]
        public string offer_id { get; set; }
        [JsonProperty("updated")]
        public bool updated { get; set; }
        [JsonProperty("errors")]
        public List<object> errors { get; set; }
    }

    public class RootResp
    {
        [JsonProperty("result")]
        public List<Result> result { get; set; }
    }

}
