using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes
{
	public class WebHostingsJob
	{
		public static List<Hosts> GetHostsList()
		{
			List<Hosts> hosts = new List<Hosts>();
			using (var db = new LiteDatabase($@"{Global.folder.Path}/Globals.db"))
			{
				var col = db.GetCollection<Hosts>("Hosts");
				hosts = col.Query().ToList();
			}
			return hosts;
		}

		public static void AddNewHosts(Hosts host)
		{
			using (var db = new LiteDatabase($@"{Global.folder.Path}/Globals.db"))
			{
				var col = db.GetCollection<Hosts>("Hosts");
				col.Insert(host);
			}
		}

		public static void DelOldHosts(Hosts host)
		{
			using (var db = new LiteDatabase($@"{Global.folder.Path}/Globals.db"))
			{
				var col = db.GetCollection<Hosts>("Hosts");
				col.Delete(host.Id);
			}
		}
	}
}
