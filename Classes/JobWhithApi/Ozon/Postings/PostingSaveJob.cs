using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Остатки.Classes.JobWhithApi.Ozon.Postings
{
    public class PostingSaveJob
    {
        public static object GetPostings(string clientId, string apiKey, int offset, int limit,
            Classes.JobWhithApi.Ozon.Postings.Request.Filter filter, Classes.JobWhithApi.Ozon.Postings.Request.With with, bool IsFBO)
        {
            HttpWebRequest httpWebRequest = null;
            if (!IsFBO)
                httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-seller.ozon.ru/v3/posting/fbs/list");
            else
                httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-seller.ozon.ru/v2/posting/fbo/list");

            httpWebRequest.Headers.Add("Client-Id", clientId);
            httpWebRequest.Headers.Add("Api-Key", apiKey);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            Classes.JobWhithApi.Ozon.Postings.Request.Root rootRequest = new Classes.JobWhithApi.Ozon.Postings.Request.Root();

            rootRequest.offset = offset;
            rootRequest.limit = limit;
            rootRequest.translit = true;


            rootRequest.dir = "ASC";
            rootRequest.filter = filter;
            rootRequest.with = with;


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
                if (!IsFBO)
                    return JsonConvert.DeserializeObject<Classes.JobWhithApi.Ozon.Postings.Response.Root>(result);
                else
                    return JsonConvert.DeserializeObject<Classes.JobWhithApi.Ozon.Postings.ResponseFBO.Root>(result);
            }
        }
    }
}
