using LiteDB;
using Windows.Storage;

namespace Остатки.Classes
{
	public class DataBaseJob
	{
		public static void AddNewProduct(Product product)
		{
			var folder = ApplicationData.Current.LocalFolder;
			using (var db = new LiteDatabase($@"{folder.Path}/ProductsDB.db"))
			{
				var col = db.GetCollection<Product>("Products");
				var proverk = col.FindOne(x => x.ArticleNumberLerya == product.ArticleNumberLerya);
				if (proverk == null)
				{
					col.Insert(product);
					Message.infoList.Add("Товар добавлен!!!");
				}
				else
				{
					Message.errorsList.Add("Данный товар уже добавлен!!!");
				}
					
			}
		}

	}
}
