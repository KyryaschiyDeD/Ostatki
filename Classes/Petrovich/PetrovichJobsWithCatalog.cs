using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Остатки.Classes.JobWhithApi.PetrovichJobs;

namespace Остатки.Classes.Petrovich
{
	public class PetrovichJobsWithCatalog
	{
		public static byte[] Decompress(byte[] data)
		{
			using (var compressedStream = new MemoryStream(data))
			using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
			using (var resultStream = new MemoryStream())
			{
				zipStream.CopyTo(resultStream);
				return resultStream.ToArray();
			}
		}
		public static byte[] ReadFully(Stream input)
		{
			byte[] buffer = new byte[16 * 1024];
			using (MemoryStream ms = new MemoryStream())
			{
				int read;
				while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
				{
					ms.Write(buffer, 0, read);
				}
				return ms.ToArray();
			}
		}

		public static Root GetCatalog()
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.petrovich.ru/catalog/v2.1/sections/tree/3?city_code=msk&rf_city=%D0%9C%D0%BE%D1%81%D0%BA%D0%B2%D0%B0&client_id=pet_site");
			request.Proxy = new WebProxy(HTMLJob.proxyIp[HTMLJob.CountproxyIp], HTMLJob.proxyPort[HTMLJob.CountproxyPort]);
			HTMLJob.CountOfUserAgent++;
			HTMLJob.CountproxyIp++;
			HTMLJob.CountproxyPort++;
			request.Method = "GET";
			request.Headers["authority"] = "api.petrovich.ru";
			request.Headers["scheme"] = "https";
			request.Headers["path"] = "  /catalog/v2.1/sections/tree/3?city_code=msk&rf_city=%D0%9C%D0%BE%D1%81%D0%BA%D0%B2%D0%B0&client_id=pet_site";
			request.Headers["accept"] = "*/*";
			request.Headers["accept-encoding"] = "gzip, deflate, br";
			request.Headers["accept-language"] = "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7";
			request.Headers["dnt"] = "1";
			request.Headers["origin"] = "https://moscow.petrovich.ru";
			request.Headers["referer"] = "https://moscow.petrovich.ru/";
			//request.Headers["sec-ch-ua-mobile"] = "?0";
			//request.Headers["sec-ch-ua-platform"] = "Windows";
			//request.Headers["sec-fetch-dest"] = "empty";
			//request.Headers["sec-fetch-mode"] = "cors";
			//request.Headers["sec-fetch-site"] = "same-site";
			request.Headers["cookie"] = "SNK=122; u__typeDevice=desktop; u__geoUserChoose=1; u__cityCode=msk; SIK=egAAAKUqxmCQ76wQF84EAA; SIV=1; C_A_R1UVF_aj8sUJ54vcfcoOF_f-s=AAAAAAAACEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA8D8AAACyoZTpQegqutni01bD_VmAWzm-_Ew; ssaid=a3f4f030-4400-11ec-b372-d747098273e1; dd_custom.lt15=2021-11-12T21:37:03.627Z; dd_custom.ts16={%22ttl%22:2592000%2C%22granularity%22:86400%2C%22data%22:{%221636675200%22:2}}; dd_user.everLoggedIn=true; dd_user.email=mic.makarov@mail.ru; dd_custom.ts8={%22ttl%22:2592000%2C%22granularity%22:86400%2C%22data%22:{%221636675200%22:3}}; dd_custom.lt7=2021-11-12T21:54:38.187Z; dd_custom.ts4={%22ttl%22:2592000%2C%22granularity%22:86400%2C%22data%22:{%221636675200%22:1}}; dd_custom.lt3=2021-11-12T21:54:47.309Z; dd_custom.ts6={%22ttl%22:2592000%2C%22granularity%22:86400%2C%22data%22:{%221636675200%22:2}}; dd_custom.lt5=2021-11-12T21:56:44.062Z; dd_custom.lastViewedProductImages=[%224579765%22%2C%227064829%22%2C%221703280%22]; dd_custom.ts12={%22ttl%22:2592000%2C%22granularity%22:86400%2C%22data%22:{%221636675200%22:16}}; dd_custom.lt11=2021-11-12T22:35:47.495Z; u__geoCityGuid=b835705e-037e-11e4-9b63-00259038e9f2; dd_user.isReturning=true; dd__persistedKeys=[%22custom.lastViewedProductImages%22%2C%22custom.lt15%22%2C%22custom.ts16%22%2C%22custom.ts12%22%2C%22custom.lt11%22%2C%22custom.lt13%22%2C%22custom.ts14%22%2C%22user.everLoggedIn%22%2C%22user.email%22%2C%22custom.ts8%22%2C%22custom.lt7%22%2C%22custom.ts6%22%2C%22custom.lt5%22%2C%22custom.ts4%22%2C%22custom.lt3%22%2C%22user.isReturning%22]; dd_custom.lt13=2021-11-13T22:04:16.641Z; dd_custom.ts14={%22ttl%22:2592000%2C%22granularity%22:86400%2C%22data%22:{%221636675200%22:29%2C%221636761600%22:3}}; __tld__=null; dd__lastEventTimestamp=1636841056673";
			request.Headers["user-agent"] = HTMLJob.userAgent[HTMLJob.CountOfUserAgent];
			HTMLJob.CountOfUserAgent++;

			var jOBj = new JObject();
			var response = (HttpWebResponse)request.GetResponse();
			Stream resStream = response.GetResponseStream();
			var t = ReadFully(resStream);
			var y = Decompress(t);
			using (var ms = new MemoryStream(y))
			using (var streamReader = new StreamReader(ms))
			using (var jsonReader = new JsonTextReader(streamReader))
			{
				jOBj = (JObject)JToken.ReadFrom(jsonReader);
			}
			//return new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")).ReadToEnd();
			Root ob = jOBj.ToObject<Root>();
			/*string str = "";
			int count = 1;
			foreach (var item in ob.data.products)
			{
				str += count++.ToString() + ": " + item.title + "\n";
			}
			return str; */
			return ob;
		}

		public static Root GetCatalogDop(string code, int offset)
		{
			string strReq = "";
			if (offset == 0)
				strReq = $@"api.petrovich.ru/catalog/v2.1/sections/{code}?limit=20&offset=0&sort=popularity_desc&path=%2Fcatalog%2F{code}%2F&city_code=msk&rf_city=%D0%9C%D0%BE%D1%81%D0%BA%D0%B2%D0%B0&client_id=pet_site";
			else
				strReq = $@"api.petrovich.ru/catalog/v2.1/sections/{code}?limit=20&offset={offset}&sort=popularity_desc&path=%2Fcatalog%2F{code}%2F&city_code=msk&rf_city=%D0%9C%D0%BE%D1%81%D0%BA%D0%B2%D0%B0&client_id=pet_site";
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://" + strReq);
			request.Proxy = new WebProxy(HTMLJob.proxyIp[HTMLJob.CountproxyIp], HTMLJob.proxyPort[HTMLJob.CountproxyPort]);
			HTMLJob.CountOfUserAgent++;
			HTMLJob.CountproxyIp++;
			HTMLJob.CountproxyPort++;
			request.Method = "GET";
			request.Headers["authority"] = "api.petrovich.ru";
			request.Headers["scheme"] = "https";
			if (offset == 0)
				request.Headers["path"] = $"/catalog/v2.1/sections/{code}?limit=20&offset=0&sort=popularity_desc&path=%2Fcatalog%2F{code}%2F&city_code=msk&rf_city=%D0%9C%D0%BE%D1%81%D0%BA%D0%B2%D0%B0&client_id=pet_site";
			else
				request.Headers["path"] = $"/catalog/v2.1/sections/{code}?offset={offset}&limit=20&sort=popularity_desc&path=%2Fcatalog%2F{code}%2F&city_code=msk&rf_city=%D0%9C%D0%BE%D1%81%D0%BA%D0%B2%D0%B0&client_id=pet_site";
			request.Headers["accept"] = "*/*";
			request.Headers["accept-encoding"] = "gzip, deflate, br";
			request.Headers["accept-language"] = "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7";
			request.Headers["dnt"] = "1";
			request.Headers["origin"] = "https://moscow.petrovich.ru";
			request.Headers["referer"] = "https://moscow.petrovich.ru/";
			request.Headers["cookie"] = "SNK=122; u__typeDevice=desktop; u__geoUserChoose=1; u__cityCode=msk; SIK=egAAAKUqxmCQ76wQF84EAA; SIV=1; C_A_R1UVF_aj8sUJ54vcfcoOF_f-s=AAAAAAAACEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA8D8AAACyoZTpQegqutni01bD_VmAWzm-_Ew; ssaid=a3f4f030-4400-11ec-b372-d747098273e1; dd_custom.lt15=2021-11-12T21:37:03.627Z; dd_custom.ts16={%22ttl%22:2592000%2C%22granularity%22:86400%2C%22data%22:{%221636675200%22:2}}; dd_user.everLoggedIn=true; dd_user.email=mic.makarov@mail.ru; dd_custom.ts8={%22ttl%22:2592000%2C%22granularity%22:86400%2C%22data%22:{%221636675200%22:3}}; dd_custom.lt7=2021-11-12T21:54:38.187Z; dd_custom.ts4={%22ttl%22:2592000%2C%22granularity%22:86400%2C%22data%22:{%221636675200%22:1}}; dd_custom.lt3=2021-11-12T21:54:47.309Z; dd_custom.ts6={%22ttl%22:2592000%2C%22granularity%22:86400%2C%22data%22:{%221636675200%22:2}}; dd_custom.lt5=2021-11-12T21:56:44.062Z; dd_custom.lastViewedProductImages=[%224579765%22%2C%227064829%22%2C%221703280%22]; dd_custom.ts12={%22ttl%22:2592000%2C%22granularity%22:86400%2C%22data%22:{%221636675200%22:16}}; dd_custom.lt11=2021-11-12T22:35:47.495Z; u__geoCityGuid=b835705e-037e-11e4-9b63-00259038e9f2; dd_user.isReturning=true; dd__persistedKeys=[%22custom.lastViewedProductImages%22%2C%22custom.lt15%22%2C%22custom.ts16%22%2C%22custom.ts12%22%2C%22custom.lt11%22%2C%22custom.lt13%22%2C%22custom.ts14%22%2C%22user.everLoggedIn%22%2C%22user.email%22%2C%22custom.ts8%22%2C%22custom.lt7%22%2C%22custom.ts6%22%2C%22custom.lt5%22%2C%22custom.ts4%22%2C%22custom.lt3%22%2C%22user.isReturning%22]; dd_custom.lt13=2021-11-13T22:04:16.641Z; dd_custom.ts14={%22ttl%22:2592000%2C%22granularity%22:86400%2C%22data%22:{%221636675200%22:29%2C%221636761600%22:3}}; __tld__=null; dd__lastEventTimestamp=1636841056673";
			request.Headers["user-agent"] = HTMLJob.userAgent[HTMLJob.CountOfUserAgent];
			HTMLJob.CountOfUserAgent++;

			var jOBj = new JObject();
			var response = (HttpWebResponse)request.GetResponse();
			Stream resStream = response.GetResponseStream();
			var t = ReadFully(resStream);
			var y = Decompress(t);
			using (var ms = new MemoryStream(y))
			using (var streamReader = new StreamReader(ms))
			using (var jsonReader = new JsonTextReader(streamReader))
			{
				jOBj = (JObject)JToken.ReadFrom(jsonReader);
			}
			Root ob = jOBj.ToObject<Root>();

			return ob;
		}
		public static Root GetProduct(string productCode)
		{
			string strReq = $@"api.petrovich.ru/catalog/v2.1/products/{productCode}?city_code=msk&rf_city=%D0%9C%D0%BE%D1%81%D0%BA%D0%B2%D0%B0&client_id=pet_site";
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://" + strReq);
			request.Proxy = new WebProxy(HTMLJob.proxyIp[HTMLJob.CountproxyIp], HTMLJob.proxyPort[HTMLJob.CountproxyPort]);
			HTMLJob.CountOfUserAgent++;
			HTMLJob.CountproxyIp++;
			HTMLJob.CountproxyPort++;
			request.Method = "GET";
			request.Headers["authority"] = "api.petrovich.ru";
			request.Headers["scheme"] = "https";
			request.Headers["path"] = $"/catalog/v2.1/products/{productCode}?city_code=msk&rf_city=%D0%9C%D0%BE%D1%81%D0%BA%D0%B2%D0%B0&client_id=pet_sit";
			request.Headers["accept"] = "*/*";
			request.Headers["accept-encoding"] = "gzip, deflate, br";
			request.Headers["accept-language"] = "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7";
			request.Headers["dnt"] = "1";
			request.Headers["origin"] = "https://moscow.petrovich.ru";
			request.Headers["referer"] = "https://moscow.petrovich.ru/";
			request.Headers["cookie"] = "SNK=122; u__typeDevice=desktop; u__geoUserChoose=1; u__cityCode=msk; SIK=egAAAKUqxmCQ76wQF84EAA; SIV=1; C_A_R1UVF_aj8sUJ54vcfcoOF_f-s=AAAAAAAACEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA8D8AAACyoZTpQegqutni01bD_VmAWzm-_Ew; ssaid=a3f4f030-4400-11ec-b372-d747098273e1; dd_custom.lt15=2021-11-12T21:37:03.627Z; dd_custom.ts16={%22ttl%22:2592000%2C%22granularity%22:86400%2C%22data%22:{%221636675200%22:2}}; dd_user.everLoggedIn=true; dd_user.email=mic.makarov@mail.ru; dd_custom.ts8={%22ttl%22:2592000%2C%22granularity%22:86400%2C%22data%22:{%221636675200%22:3}}; dd_custom.lt7=2021-11-12T21:54:38.187Z; dd_custom.ts4={%22ttl%22:2592000%2C%22granularity%22:86400%2C%22data%22:{%221636675200%22:1}}; dd_custom.lt3=2021-11-12T21:54:47.309Z; dd_custom.ts6={%22ttl%22:2592000%2C%22granularity%22:86400%2C%22data%22:{%221636675200%22:2}}; dd_custom.lt5=2021-11-12T21:56:44.062Z; dd_custom.lastViewedProductImages=[%224579765%22%2C%227064829%22%2C%221703280%22]; dd_custom.ts12={%22ttl%22:2592000%2C%22granularity%22:86400%2C%22data%22:{%221636675200%22:16}}; dd_custom.lt11=2021-11-12T22:35:47.495Z; u__geoCityGuid=b835705e-037e-11e4-9b63-00259038e9f2; dd_user.isReturning=true; dd__persistedKeys=[%22custom.lastViewedProductImages%22%2C%22custom.lt15%22%2C%22custom.ts16%22%2C%22custom.ts12%22%2C%22custom.lt11%22%2C%22custom.lt13%22%2C%22custom.ts14%22%2C%22user.everLoggedIn%22%2C%22user.email%22%2C%22custom.ts8%22%2C%22custom.lt7%22%2C%22custom.ts6%22%2C%22custom.lt5%22%2C%22custom.ts4%22%2C%22custom.lt3%22%2C%22user.isReturning%22]; dd_custom.lt13=2021-11-13T22:04:16.641Z; dd_custom.ts14={%22ttl%22:2592000%2C%22granularity%22:86400%2C%22data%22:{%221636675200%22:29%2C%221636761600%22:3}}; __tld__=null; dd__lastEventTimestamp=1636841056673";
			request.Headers["user-agent"] = HTMLJob.userAgent[HTMLJob.CountOfUserAgent];
			HTMLJob.CountOfUserAgent++;

			var jOBj = new JObject();
			HttpWebResponse response = null;

			try
            {
				response = (HttpWebResponse)request.GetResponse();
			}
            catch (Exception)
            {
				return null;
            }
			
			Stream resStream = response.GetResponseStream();
			var t = ReadFully(resStream);
			var y = Decompress(t);
			using (var ms = new MemoryStream(y))
			using (var streamReader = new StreamReader(ms))
			using (var jsonReader = new JsonTextReader(streamReader))
			{
				jOBj = (JObject)JToken.ReadFrom(jsonReader);
			}
			Root ob = jOBj.ToObject<Root>();

			return ob;
		}

		public static int GetRemainsBlack(Remains remains)
        {
			int remainsBlack = 0;
			if (remains.supply_ways != null)
				foreach (var supply_ways in remains.supply_ways)
				{
					if (supply_ways.subdivision_list != null)
						foreach (var item in supply_ways.subdivision_list)
						{
							remainsBlack += item.remains_amount;
						}
				}
			return remainsBlack - remains.total;

		}
	}
}
