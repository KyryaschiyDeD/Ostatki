using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Остатки.Classes.JobWhithApi.Ozon.ProductInfo;

namespace Остатки.Classes.JobWhithApi.Ozon
{
    public class CommissionsByVolumetricWeight
    {
        public double procent { get; set; }
        public int min { get; set; }
        public int max { get; set; }
    }
    public class GetProductInfo
    {
        public static ProductInfoFromOzon PostRequestAsync(string clientId, string apiKey, List<string> offerIds)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api-seller.ozon.ru/v2/product/info/list");
            httpWebRequest.Headers.Add("Client-Id", clientId);
            httpWebRequest.Headers.Add("Api-Key", apiKey);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            Request request = new Request();
            request.offer_id = offerIds;

            var jsort = JsonConvert.SerializeObject(request);
            using (var requestStream = httpWebRequest.GetRequestStream())
            using (var writer = new StreamWriter(requestStream))
            {
                writer.Write(jsort);
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                //ответ от сервера
                var result = streamReader.ReadToEnd();

                //Сериализация
                return JsonConvert.DeserializeObject<ProductInfoFromOzon>(result);
            }
        }

        public static CommissionsByVolumetricWeight CommissionByVolumetricWeight(double volumetricWeight)
        {
            if (volumetricWeight < 0.1)
                return null;
            switch (volumetricWeight)
            {
                case 0.1:
                    return new CommissionsByVolumetricWeight() { procent = 4, min = 41, max = 50 };
                case 0.2:
                    return new CommissionsByVolumetricWeight() { procent = 4, min = 42, max = 50 };
                case 0.3:
                    return new CommissionsByVolumetricWeight() { procent = 4, min = 43, max = 60 };
                case 0.4:
                    return new CommissionsByVolumetricWeight() { procent = 4, min = 45, max = 65 };
                case 0.5:
                    return new CommissionsByVolumetricWeight() { procent = 4, min = 47, max = 70 };
                case 0.6:
                    return new CommissionsByVolumetricWeight() { procent = 4, min = 50, max = 70 };
                case 0.7:
                    return new CommissionsByVolumetricWeight() { procent = 4, min = 53, max = 75 };
                case 0.8:
                    return new CommissionsByVolumetricWeight() { procent = 4, min = 55, max = 75 };
                case 0.9:
                    return new CommissionsByVolumetricWeight() { procent = 4, min = 55, max = 80 };
                case 1.0:
                    return new CommissionsByVolumetricWeight() { procent = 5, min = 57, max = 95 };
                case 1.1:
                    return new CommissionsByVolumetricWeight() { procent = 5, min = 59, max = 95 };
                case 1.2:
                    return new CommissionsByVolumetricWeight() { procent = 5, min = 63, max = 100 };
                case 1.3:
                    return new CommissionsByVolumetricWeight() { procent = 5, min = 63, max = 105 };
                case 1.4:
                    return new CommissionsByVolumetricWeight() { procent = 5, min = 67, max = 105 };
                case 1.5:
                    return new CommissionsByVolumetricWeight() { procent = 5, min = 67, max = 125 };
                case 1.6:
                    return new CommissionsByVolumetricWeight() { procent = 5, min = 70, max = 125 };
                case 1.7:
                    return new CommissionsByVolumetricWeight() { procent = 5, min = 71, max = 125 };
                case 1.8:
                    return new CommissionsByVolumetricWeight() { procent = 5, min = 75, max = 130 };
                case 1.9:
                    return new CommissionsByVolumetricWeight() { procent = 5, min = 77, max = 130 };
            }
            if (2 <= volumetricWeight && volumetricWeight <= 2.9)
                return new CommissionsByVolumetricWeight() { procent = 5, min = 77, max = 130 };
            if (3 <= volumetricWeight && volumetricWeight <= 3.9)
                return new CommissionsByVolumetricWeight() { procent = 5.5, min = 115, max = 175 };
            if (4 <= volumetricWeight && volumetricWeight <= 4.9)
                return new CommissionsByVolumetricWeight() { procent = 5.5, min = 155, max = 215 };
            if (5 <= volumetricWeight && volumetricWeight <= 5.9)
                return new CommissionsByVolumetricWeight() { procent = 5.5, min = 175, max = 275 };
            if (5 <= volumetricWeight && volumetricWeight <= 5.9)
                return new CommissionsByVolumetricWeight() { procent = 5.5, min = 175, max = 275 };
            if (6 <= volumetricWeight && volumetricWeight <= 6.9)
                return new CommissionsByVolumetricWeight() { procent = 5.5, min = 200, max = 315 };
            if (7 <= volumetricWeight && volumetricWeight <= 7.9)
                return new CommissionsByVolumetricWeight() { procent = 5.5, min = 215, max = 350 };
            if (8 <= volumetricWeight && volumetricWeight <= 8.9)
                return new CommissionsByVolumetricWeight() { procent = 5.5, min = 245, max = 385 };
            if (9 <= volumetricWeight && volumetricWeight <= 9.9)
                return new CommissionsByVolumetricWeight() { procent = 5.5, min = 270, max = 395 };
            if (10 <= volumetricWeight && volumetricWeight <= 10.9)
                return new CommissionsByVolumetricWeight() { procent = 6, min = 300, max = 400 };
            if (11 <= volumetricWeight && volumetricWeight <= 11.9)
                return new CommissionsByVolumetricWeight() { procent = 6, min = 315, max = 450 };
            if (12 <= volumetricWeight && volumetricWeight <= 12.9)
                return new CommissionsByVolumetricWeight() { procent = 6, min = 345, max = 490 };
            if (13 <= volumetricWeight && volumetricWeight <= 13.9)
                return new CommissionsByVolumetricWeight() { procent = 6, min = 365, max = 510 };
            if (14 <= volumetricWeight && volumetricWeight <= 14.9)
                return new CommissionsByVolumetricWeight() { procent = 6, min = 400, max = 515 };
            if (15 <= volumetricWeight && volumetricWeight <= 19.9)
                return new CommissionsByVolumetricWeight() { procent = 6, min = 485, max = 550 };
            if (20 <= volumetricWeight && volumetricWeight <= 24.9)
                return new CommissionsByVolumetricWeight() { procent = 6, min = 585, max = 650 };
            if (25 <= volumetricWeight)
                return new CommissionsByVolumetricWeight() { procent = 8, min = 650, max = 650 };
            return null;
        }
    }
}
