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
using Остатки.Classes.JobWhithApi.Ozon.StockUpdate;
using Остатки.Classes.ProductsClasses.InfoPrices;

namespace Остатки.Classes.JobWhithApi.Ozon.PriceUpdate
{
    public class Price
    {
        [JsonProperty("min_price")]
        public string min_price { get; set; }

        [JsonProperty("offer_id")]
        public string offer_id { get; set; }

        [JsonProperty("old_price")]
        public string old_price { get; set; }

        [JsonProperty("price")]
        public string price { get; set; }

        [JsonProperty("product_id")]
        public int product_id { get; set; }
    }

    public class Root
    {
        [JsonProperty("prices")]
        public List<Price> prices { get; set; }
    }
    public class Prices
    {

        public static bool ChechNewPriceOfFiveProcent(int a, string str)
        {
            double b;
            bool ctr = double.TryParse(str.Replace('.',','), out b);

            if (a > b)
            {
                if (((double)a / b - 1) >= 0.05)
                    return true;
                else
                    return false;
            }
            else
            {
                if ((b / (double)a - 1) >= 0.05)
                    return true;
                else
                    return false;
            }
        }

        public static bool CheckNewPriceOfMaximumAndMax(int a, string str)
        {
            double b;
            bool ctr = double.TryParse(str.Replace('.', ','), out b);

            if (b <= (double)a)
                return true;
            else if ((b / (double)a - 1) <= 0.055)
                return true;
            return false;
        }

        public static async void GoUpdatePrices(List<Product> products)
        {
            List<Price> prices = new List<Price>();
            int countToUpdatePrice = 0;
            List<RootResp> resp = new List<RootResp>();
            List<ApiKeys> allApi = ApiKeysesJob.GetAllApiList();
            List<ApiKeys> apiWhithMaxPrice = allApi.Where(x => x.IsTheMaximumPrice == true).ToList();
            List<Product> NoInfoPrice = new List<Product>();
            Dictionary<ApiKeys, List<Price>> updatePriceOnMaxApiAccaunt = new Dictionary<ApiKeys, List<Price>>();

            foreach (var keys in allApi)
            {
                if (keys.IsPriceUpdate)
                {
                    List<ArticleNumber> DellFromSale = new List<ArticleNumber>();
                    foreach (Product product in products)
                    {
                        if (product.ArticleNumberProductId.ContainsKey(keys.ClientId) && product.ArticleNumberProductId[keys.ClientId].Count != 0)
                        {
                            if (product.Weight == 0)
                                product.Weight = 5000;

                            int kolKompl = 0;
                            foreach (var item in product.ArticleNumberProductId[keys.ClientId])
                            {
                                if (item.OurArticle.Contains("x10"))
                                    kolKompl = 10;
                                else
                                if (item.OurArticle.Contains("x5"))
                                    kolKompl = 5;
                                else
                                if (item.OurArticle.Contains("x3"))
                                    kolKompl = 3;
                                else
                                if (item.OurArticle.Contains("x2"))
                                    kolKompl = 2;
                                else
                                    kolKompl = 1;

                                int newPrice = 0;
                                double koef = 0;

                                if (product.NowPrice * kolKompl <= 10)
                                {
                                    koef = 5;
                                }
                                else
                                if (product.NowPrice * kolKompl <= 20)
                                {
                                    koef = 4;
                                }
                                else
                                if (product.NowPrice * kolKompl < 40)
                                {
                                    koef = 3;
                                }
                                else
                                if (product.NowPrice * kolKompl < 100)
                                {
                                    koef = 2.2;
                                }
                                else
                                if (product.NowPrice * kolKompl < 200)
                                {
                                    koef = 1.7;
                                }
                                else
                                if (product.NowPrice * kolKompl < 500)
                                {
                                    koef = 1.35;
                                }
                                else
                                if (product.NowPrice * kolKompl < 1000)
                                {
                                    koef = 1.2;
                                }
                                else
                                    koef = 1;

                                bool ItIsNewPrice = false;

                                if (item.productInfoPriceFromOzon != null)
                                {
                                    if (item.productInfoPriceFromOzon.sales_percent == 0)
                                    {
                                        NoInfoPrice.Add(product);
                                        continue;
                                        /*Thread.Sleep(500);
                                        RootInfoPrices rootInfoPrices = GetProductPriceInfo.PostRequestAsyncWhithList(keys.ClientId, keys.ApiKey, "", 1, new List<string>() { item.OurArticle });
                                        if (rootInfoPrices.result.items.Count != 0)
                                        {
                                            newPrice = Convert.ToInt32((Convert.ToDouble(product.NowPrice) * kolKompl * koef * 1.15 +
                                            (rootInfoPrices.result.items.First().commissions.fbs_first_mile_max_amount + rootInfoPrices.result.items.First().commissions.fbs_direct_flow_trans_max_amount
                                            + rootInfoPrices.result.items.First().commissions.fbs_deliv_to_customer_amount)) * 1.05 / (100 - rootInfoPrices.result.items.First().commissions.sales_percent) * 100) / 10 * 10;
                                            ItIsNewPrice = true;
                                        }*/
                                    }
                                    else
                                    {
                                        newPrice = Convert.ToInt32((Convert.ToDouble(product.NowPrice) * kolKompl * koef * 1.20 / 93 * 100 +
                                            (item.productInfoPriceFromOzon.fbs_first_mile_max_amount + item.productInfoPriceFromOzon.fbs_direct_flow_trans_max_amount
                                            + item.productInfoPriceFromOzon.fbs_deliv_to_customer_amount)) * 1.05 / (100 - item.productInfoPriceFromOzon.sales_percent) * 100 + 25) / 10 * 10;
                                        ItIsNewPrice = true;
                                    }
                                }
                                foreach (var keyMaxPrice in apiWhithMaxPrice)
                                {
                                    if (product.ArticleNumberProductId.ContainsKey(keyMaxPrice.ClientId))
                                    {
                                        foreach (var oneMaxProduct in product.ArticleNumberProductId[keyMaxPrice.ClientId])
                                        {
                                            if (
                                                CheckNewPriceOfMaximumAndMax
                                                    (
                                                    newPrice,
                                                    oneMaxProduct.productInfoFromOzon.price
                                                    )
                                                )
                                            {
                                                if (!updatePriceOnMaxApiAccaunt.ContainsKey(keyMaxPrice))
                                                {
                                                    updatePriceOnMaxApiAccaunt.Add(keyMaxPrice, new List<Price>());
                                                }

                                                updatePriceOnMaxApiAccaunt[keyMaxPrice].Add
                                                    (
                                                    new Price()
                                                    {
                                                        product_id = (int)oneMaxProduct.ArticleOzon,
                                                        price = (newPrice * 1.1).ToString(),
                                                        old_price = (newPrice * 1.5).ToString()
                                                    }
                                                    );
                                            }

                                        }
                                    }
                                }
                                if (ItIsNewPrice)
                                {
                                    if (ChechNewPriceOfFiveProcent(newPrice, item.productInfoFromOzon.price))
                                    {
                                        prices.Add(new Price() { product_id = (int)item.ArticleOzon, price = newPrice.ToString(), old_price = (newPrice * 1.4).ToString() });
                                        countToUpdatePrice++;
                                    }
                                }
                                else
                                {
                                    DellFromSale.Add(item);
                                }

                                if (countToUpdatePrice >= 990)
                                {
                                    countToUpdatePrice = 0;
                                    resp.Add(SendReqwest(keys.ClientId, keys.ApiKey, prices));
                                    foreach (var oneMaxUpdate in updatePriceOnMaxApiAccaunt)
                                    {
                                        resp.Add(SendReqwest(oneMaxUpdate.Key.ClientId, oneMaxUpdate.Key.ApiKey, oneMaxUpdate.Value));
                                    }
                                    updatePriceOnMaxApiAccaunt.Clear();
                                    prices.Clear();
                                }
                            }
                        }
                    }

                    Stocks.GoUpdateStocksToNull(DellFromSale, keys);
                    countToUpdatePrice = 0;
                    DellFromSale.Clear();

                    foreach (var oneMaxUpdate in updatePriceOnMaxApiAccaunt)
                    {
                        resp.Add(SendReqwest(oneMaxUpdate.Key.ClientId, oneMaxUpdate.Key.ApiKey, oneMaxUpdate.Value));
                    }
                    updatePriceOnMaxApiAccaunt.Clear();
                    prices.Clear();

                }
            }

            

            string str = "";
            int kolErrors = 0;
            List<string> errorsOfferID = new List<string>();
            foreach (var result in resp)
            {
                foreach (var oneResult in result.result)
                {
                    if (!oneResult.updated)
                    {
                        errorsOfferID.Add(oneResult.offer_id);
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
                await fileWithLinks.CreateFileAsync("Ошибка обновления.txt", CreationCollisionOption.ReplaceExisting);
                StorageFile myFile = await fileWithLinks.GetFileAsync("Ошибка обновления.txt");
                await FileIO.WriteTextAsync(myFile, str);
            }
            using (var db = new LiteDatabase($@"{Global.folder.Path}/ProductsDB.db"))
            {
                var col = db.GetCollection<Product>("Products");
                List<Product> Remains = col.Query().ToList();
                foreach (var product in Remains)
                {
                    bool update = true;
                    foreach (var offer in product.ArticleNumberUnicList)
                    {
                        if (errorsOfferID.Contains(offer))
                            update = false;
                    }
                    if (update)
                    {
                        product.NewPriceIsSave = true;
                        col.Update(product);
                    }
                    
                }
            }
        }

        private static RootResp SendReqwest(string clientId, string apiKey, List<Price> prices)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-seller.ozon.ru/v1/product/import/prices");
            httpWebRequest.Headers.Add("Client-Id", clientId);
            httpWebRequest.Headers.Add("Api-Key", apiKey);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            Root rootRequest = new Root();
            rootRequest.prices = prices;


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
    }

    public class RespResult
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
        public List<RespResult> result { get; set; }
    }


}
