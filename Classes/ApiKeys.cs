using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes
{
	public class ApiKeys
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string ClientId { get; set; }
		public string ApiKey { get; set; }
		public bool ItIsTop { get; set; }	
		public bool InDB { get; set; }	
		public bool IsOstatkiUpdate { get; set; }
		public bool IsPriceUpdate { get; set; }
		public bool IsTheMaximumPrice { get; set; }
		public int MaxCountTopProduct { get; set; }	
		public DateTime DateCreate {get; set;}
	}
}
