﻿using LiteDB;
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
        public static async void GoUpdatePrices(List<Product> products)
        {
            List<Price> prices = new List<Price>();
            int countToUpdatePrice = 0;
            List<RootResp> resp = new List<RootResp>();
            foreach (var keys in ApiKeysesJob.GetAllApiList())
            {
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

                            int newPrice = (int)(Convert.ToInt32(Convert.ToDouble(product.NowPrice) * kolKompl + 45 + product.Weight / 1000 * 20 + 50) * 1.075 * 1.1 * 1.25 * 1.1 + 5) / 10 * 10;

                            prices.Add(new Price() { product_id = (int)item.ArticleOzon, price = newPrice.ToString(), old_price = (newPrice * 1.4).ToString() });
                            countToUpdatePrice++;

                            if (countToUpdatePrice >= 450)
                            {
                                countToUpdatePrice = 0;
                                resp.Add(SendReqwest(keys.ClientId, keys.ApiKey, prices));
                                prices = new List<Price>();
                            }
                        }
                    }
                }
                countToUpdatePrice = 0;
                resp.Add(SendReqwest(keys.ClientId, keys.ApiKey, prices));
                prices = new List<Price>();
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
