using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Остатки.Classes.Taxation.TaxClasses;

namespace Остатки.Classes.Taxation
{
    public class Proverkacheka
    {
        /// <summary>
        /// Токен от сервиса proverkacheka.com
        /// </summary>
        private readonly string apiToken;
        /// <summary>
        /// Ссылка на API сервиса
        /// </summary>
        private static readonly string url = "https://proverkacheka.com/api/v1/check/get";
        private static readonly HttpClient client = new HttpClient();

        /// <summary>
        /// Инициализация класса
        /// </summary>
        /// <param name="apiToken">Токен из proverkacheka.com</param>
        public Proverkacheka(string apiToken)
        {
            this.apiToken = apiToken;
        }

        public Task<Receipt> GetAsyncByRaw(string qrRaw)
        {
            return GetAsyncByRaw(apiToken, qrRaw);
        }

        public Task<Receipt> GetAsyncByFile(string filepath)
        {
            return GetAsyncByFile(apiToken, filepath);
        }

        public static async Task<Receipt> GetAsyncByRaw(string apiToken, string qrRaw)
        {
            var values = new Dictionary<string, string>
            {
                { "token", apiToken},
                { "qrraw", qrRaw }
            };
            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync(url, content);
            var json = JObject.Parse(await response.Content.ReadAsStringAsync());

            return ConvertJsonToReceipt(json);
        }

        public static async Task<Receipt> GetAsyncByFile(string apiToken, string filepath)
        {
            var content = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent(System.IO.File.ReadAllBytes(filepath));

            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("qrfile")
            {
                FileName = $"qr.{filepath.Split('.').Last()}"
            };
            content.Add(new StringContent(apiToken), "token");
            content.Add(fileContent);

            var response = await client.PostAsync(url, content);
            var json = JObject.Parse(await response.Content.ReadAsStringAsync());

            return ConvertJsonToReceipt(json);
        }

        private static Receipt ConvertJsonToReceipt(JObject json)
        {
            switch (json["code"].ToString())
            {
                case "3":
                    return new Receipt(json["data"].ToString());
            }

            JObject data = (JObject)json["data"]["json"];

            return JsonConvert.DeserializeObject<Receipt>(data.ToString());
        }

    }
}
