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
		public static void CreateNewApi(string name, string clientId, string apiKey)
		{
			using (var db = new LiteDatabase($@"{Global.folder.Path}/Globals.db"))
			{
				var col = db.GetCollection<ApiKeys>("ApiKeyses");
				var proverk = col.FindOne(x => x.ApiKey == apiKey);
				if (proverk == null)
					col.Insert(new ApiKeys() { Name = name, ClientId = clientId, ApiKey = apiKey, DateCreate = DateTime.Now });
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
		public static string GetApiName(ApiKeys pf)
		{
			return pf.Name;
		}
	}
}
