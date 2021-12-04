using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Остатки.Classes.JobWhithApi.PetrovichJobs;
using Остатки.Classes.Petrovich;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace Остатки.Pages
{
	/// <summary>
	/// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
	/// </summary>
	public sealed partial class PetrovichPage : Page
	{
		private void PetrovichProduct_Click(object sender, RoutedEventArgs e)
		{
			GridPetrovich.Children.Clear();
			Classes.JobWhithApi.PetrovichJobs.Root oneProduct = PetrovichJobsWithCatalog.GetProduct((sender as Button).Tag.ToString());
			string info = oneProduct.data.product.title + "\n";
			TextBlock textBlock = new TextBlock();
			textBlock.Text = info;
			GridPetrovich.Children.Add(textBlock);
		}
		private void PetrovichCategory_Click(object sender, RoutedEventArgs e)
		{
			GridPetrovich.Children.Clear();
			Classes.JobWhithApi.PetrovichJobs.Root nextCat = PetrovichJobsWithCatalog.GetCatalogDop((sender as Button).Tag.ToString(), 0);
			if (nextCat.data.section.children != null)
			{
				foreach (var item in nextCat.data.section.children)
				{
					Button button = new Button();
					button.Content = item.title + ": " + item.product_qty;
					button.CornerRadius = new CornerRadius() { BottomLeft = 15, BottomRight = 15, TopLeft = 15, TopRight = 15 };
					button.Margin = new Thickness() { Bottom = 0, Left = 0, Right = 0, Top = 10 };
					button.Click += PetrovichCategory_Click;
					button.Tag = item.code;
					GridPetrovich.Children.Add(button);
				}
			}
			else
			{
				int countProduct = nextCat.data.pagination.products_count;
				countProduct -= 20;
				int skippedCountProduct = 20;
				while (countProduct > 0)
				{
					foreach (var OneProduct in nextCat.data.products)
					{
						Button button = new Button();
						button.Content = OneProduct.title;
						button.CornerRadius = new CornerRadius() { BottomLeft = 15, BottomRight = 15, TopLeft = 15, TopRight = 15 };
						button.Margin = new Thickness() { Bottom = 0, Left = 0, Right = 0, Top = 10 };
						button.Click += PetrovichProduct_Click;
						button.Tag = OneProduct.code;
						GridPetrovich.Children.Add(button);
					}
					nextCat = PetrovichJobsWithCatalog.GetCatalogDop((sender as Button).Tag.ToString(), skippedCountProduct);
					skippedCountProduct += 20;
					countProduct -= 20;
				}
			}
		}

		public PetrovichPage()
		{
			this.InitializeComponent();
			Classes.JobWhithApi.PetrovichJobs.Root list = PetrovichJobsWithCatalog.GetCatalog();
			foreach (var item in list.data.sections)
			{
				Button button = new Button();
				button.Content = item.title;
				button.CornerRadius = new CornerRadius() { BottomLeft = 15, BottomRight = 15, TopLeft = 15, TopRight = 15};
				button.Margin = new Thickness() { Bottom = 0, Left = 0, Right = 0, Top = 10};
				button.Click += PetrovichCategory_Click; 
				button.Tag = item.code;
				GridPetrovich.Children.Add(button);
			}
		}
	}
}
