using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes
{
	public class ApiKeysesJob
	{
		public static List<ApiKeys> GetAllApiList()
		{
			using (var db = new LiteDatabase($@"{Global.folder.Path}/Globals.db"))
			{
				var col = db.GetCollection<ApiKeys>("ApiKeyses");
				return col.Query().ToList();
			}
		}
		public static void CreateNewApi
			(
			string name, 
			string clientId, 
			string apiKey, 
			string MaxCountTopProduct, 
			object itIsTop, 
			object inDB, 
			object isOstatkiUpdate, 
			object isPriceUpdate,
			object isTheMaximumPrice
			)
		{
			bool chIsCreate = false;
			int ch = 0;
			if (MaxCountTopProduct.Length != 0)
            {
				if (int.TryParse(MaxCountTopProduct, out ch))
					chIsCreate = true;
			}
			
			using (var db = new LiteDatabase($@"{Global.folder.Path}/Globals.db"))
			{
				var col = db.GetCollection<ApiKeys>("ApiKeyses");
				var proverk = col.FindOne(x => x.ApiKey == apiKey);
				if (proverk == null)
					col.Insert(new ApiKeys() { 
						Name = name, 
						ClientId = clientId, 
						ApiKey = apiKey, 
						DateCreate = DateTime.Now, 
						MaxCountTopProduct = ch, 
						ItIsTop = (bool)itIsTop, 
						InDB = (bool)inDB, 
						IsOstatkiUpdate = (bool)isOstatkiUpdate, 
						IsPriceUpdate = (bool)isPriceUpdate,
						IsTheMaximumPrice = (bool)isTheMaximumPrice
					});
				
					
			}
		}

		public static void ReadctOldApi
			(
			string name, 
			string clientId, 
			string apiKey
			)
		{
			using (var db = new LiteDatabase($@"{Global.folder.Path}/Globals.db"))
			{
				var col = db.GetCollection<ApiKeys>("ApiKeyses");
				var proverk = col.FindOne(x => x.ClientId == clientId);
				if (proverk != null)
                {
					col.Update(proverk);
				}
			}
		}

		public static void DeleteAllApi()
		{
			using (var db = new LiteDatabase($@"{Global.folder.Path}/Globals.db"))
			{
				var col = db.GetCollection<ApiKeys>("ApiKeyses");
				col.DeleteAll();
			}
		}
		public static void UpdateOneApi(ApiKeys newApi)
		{
			using (var db = new LiteDatabase($@"{Global.folder.Path}/Globals.db"))
			{
				var col = db.GetCollection<ApiKeys>("ApiKeyses");
				col.Update(newApi);
			}
		}
		public static List<string> GetNames()
		{
			using (var db = new LiteDatabase($@"{Global.folder.Path}/Globals.db"))
			{
				var col = db.GetCollection<ApiKeys>("ApiKeyses");
				return 
				new List<string>(col.Query().ToList().ConvertAll(new Converter<ApiKeys, string>(ApiKeysesJob.GetApiName)));
			}
		}

		public static string GetApiByKey(string clientID)
		{
			using (var db = new LiteDatabase($@"{Global.folder.Path}/Globals.db"))
			{
				var col = db.GetCollection<ApiKeys>("ApiKeyses");
				List<ApiKeys> str = col.Query().ToList().Where(x => x.ClientId.Equals(clientID)).ToList();
				return str.First().ApiKey;
			}
		}

		public static string GetApiName(ApiKeys pf)
		{
			return pf.Name;
		}
	}
}
