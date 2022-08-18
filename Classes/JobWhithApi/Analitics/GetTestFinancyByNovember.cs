using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.Analitics
{
	public class GetTestFinancyByNovember
	{
        public static TestAnalitics.Response.Root PostRequestAsync(ApiKeys key, int offset, string status)
        {
            TestAnalitics.Request.Root root = new TestAnalitics.Request.Root();
            root.dir = "ASC";
            root.filter = new TestAnalitics.Request.Filter();
            root.filter.since = Convert.ToDateTime("2022-05-25T21:00:00.000Z");
            root.filter.to = Convert.ToDateTime("2022-06-05T21:00:00.000Z");
            root.filter.status = status;
            root.limit = 1000;
            root.offset = offset;
            root.translit = true;
            root.with = new TestAnalitics.Request.With();
            root.with.financial_data = true;

            var jsort = JsonConvert.SerializeObject(root);
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-seller.ozon.ru/v3/posting/fbs/list");
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

            httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                //ответ от сервера
                var result = streamReader.ReadToEnd();
                //Сериализация
                return JsonConvert.DeserializeObject<TestAnalitics.Response.Root>(result);
            }
        }

    }
}
