using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Остатки.Classes;
using System.Linq;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace Остатки.Pages.Statistics
{
	/// <summary>
	/// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
	/// </summary>
	public sealed partial class TestFinansy : Page
	{
		public TestFinansy()
		{
			this.InitializeComponent();
		}


		private double GetKolYmnNaPrice(string offerId, double NowPrice, int quantity, ref int count)
		{
			double kolYmnNaPrice = 0;
			string[] keys = new string[] { "x10", "x5", "x3", "x2" };
			string sKeyResult = keys.FirstOrDefault(s => offerId.Contains(s));

			switch (sKeyResult)
			{
				case "x10":
					kolYmnNaPrice = NowPrice * 10 * quantity;
					count += 10* quantity;
					break;
				case "x5":
					kolYmnNaPrice = NowPrice * 5 * quantity;
					count += 5 * quantity;
					break;
				case "x3":
					kolYmnNaPrice = NowPrice * 3 * quantity;
					count += 3 * quantity;
					break;
				case "x2":
					kolYmnNaPrice = NowPrice * 2 * quantity;
					count += 2 * quantity;
					break;
				default:
					kolYmnNaPrice = NowPrice * quantity;
					count += quantity;
					break;
			} 
			return kolYmnNaPrice;
		}

		

	private void GetTestFinancy_Click(object sender, RoutedEventArgs e)
		{
			List<Product> NewProduct = new List<Product>();

			List<Product> Remains = new List<Product>();
			List<Product> Wait = new List<Product>();
			List<Product> Archive = new List<Product>();
			List<Product> Del = new List<Product>();

			List<Product> AllPrpoduct = new List<Product>();

			DataBaseJob.GetAllProductFromTheBase
			(
			out Remains,
			out Wait,
			out Archive,
			out Del
			);

			AllPrpoduct.AddRange(Remains);
			AllPrpoduct.AddRange(Wait);
			AllPrpoduct.AddRange(Archive);
			AllPrpoduct.AddRange(Del);

			StackPanel stackPanel = new StackPanel();
			foreach (var key in ApiKeysesJob.GetAllApiList())
			{
				double allSumZak = 0;
				double allPrice = 0;
				double allPrible = 0;
				double allRasxod = 0;
				double allZakyyp = 0;

				int countProduct = 0;
				int countProductOzon = 0;
				int countOtpravl = 0;
				int countVozvrat = 0;
				int countProblem = 0;

				string problem = "";
				string tovarKypl = "";
				string otpravlNumber = "";
				string artikylDataBase = "";

				int countBumaga = 0;
				double allPriceBumag = 0;

				List<Classes.JobWhithApi.Analitics.TestAnalitics.Response.Posting> listDeparture = new List<Classes.JobWhithApi.Analitics.TestAnalitics.Response.Posting>();

				List<string> statuses = new List<string>() 
				{ 
					""
				};
				foreach (var status in statuses)
				{
					bool itIsAll = true;
					int offset = 0;
					while (itIsAll)
					{
						Classes.JobWhithApi.Analitics.TestAnalitics.Response.Root res = Classes.JobWhithApi.Analitics.GetTestFinancyByNovember.PostRequestAsync(key, offset, status);
						listDeparture.AddRange(new List<Classes.JobWhithApi.Analitics.TestAnalitics.Response.Posting>(res.result.postings));
						
						offset += res.result.postings.Count();
						itIsAll = res.result.has_next;
					}
				}

				foreach (var item in listDeparture)
				{
					otpravlNumber += item.posting_number + "\n";
					countOtpravl++;
					foreach (var oneProduct in item.products)
					{
						tovarKypl += oneProduct.offer_id + " x " + oneProduct.quantity + "\n";
					}
					foreach (var OneFinancialData in item.financial_data.products)
					{
						allSumZak += Convert.ToDouble(OneFinancialData.price) * OneFinancialData.quantity;
						countProductOzon += OneFinancialData.quantity;
						if (OneFinancialData.item_services.marketplace_service_item_return_flow_trans != 0 || OneFinancialData.item_services.marketplace_service_item_return_after_deliv_to_customer != 0)
						{
							countVozvrat += OneFinancialData.quantity;
							countProduct += OneFinancialData.quantity;

							string offerId = item.products.ElementAt(item.financial_data.products.IndexOf(OneFinancialData)).offer_id;
							if (offerId.Contains("lnrd"))
							{
								offerId = offerId.Replace("lnrd", "ld");
								offerId = offerId.Replace("_", "-");
							}

							Product oneProduct = AllPrpoduct.Where(x => x.ArticleNumberUnicList.Contains(offerId) || x.ArticleNumberInShop == offerId).FirstOrDefault();
							if (oneProduct != null)
							{
								double kolYmnNaPrice = GetKolYmnNaPrice(offerId, oneProduct.NowPrice, OneFinancialData.quantity, ref countProduct);
								allZakyyp += kolYmnNaPrice;
								artikylDataBase += oneProduct.ArticleNumberInShop + "\n";
							}

								

							allPrible += OneFinancialData.item_services.marketplace_service_item_deliv_to_customer * OneFinancialData.quantity;
							allPrible += OneFinancialData.item_services.marketplace_service_item_direct_flow_trans * OneFinancialData.quantity;
							allPrible += OneFinancialData.item_services.marketplace_service_item_dropoff_ff * OneFinancialData.quantity;
							allPrible += OneFinancialData.item_services.marketplace_service_item_dropoff_pvz * OneFinancialData.quantity;
							allPrible += OneFinancialData.item_services.marketplace_service_item_dropoff_sc * OneFinancialData.quantity;
							allPrible += OneFinancialData.item_services.marketplace_service_item_fulfillment * OneFinancialData.quantity;
							allPrible += OneFinancialData.item_services.marketplace_service_item_pickup * OneFinancialData.quantity;
							allPrible += OneFinancialData.item_services.marketplace_service_item_return_after_deliv_to_customer * OneFinancialData.quantity;
							allPrible += OneFinancialData.item_services.marketplace_service_item_return_flow_trans * OneFinancialData.quantity;
							allPrible += OneFinancialData.item_services.marketplace_service_item_return_not_deliv_to_customer * OneFinancialData.quantity;
							allPrible += OneFinancialData.item_services.marketplace_service_item_return_part_goods_customer * OneFinancialData.quantity;

							allRasxod -= OneFinancialData.item_services.marketplace_service_item_deliv_to_customer * OneFinancialData.quantity;
							allRasxod -= OneFinancialData.item_services.marketplace_service_item_direct_flow_trans * OneFinancialData.quantity;
							allRasxod -= OneFinancialData.item_services.marketplace_service_item_dropoff_ff * OneFinancialData.quantity;
							allRasxod -= OneFinancialData.item_services.marketplace_service_item_dropoff_pvz * OneFinancialData.quantity;
							allRasxod -= OneFinancialData.item_services.marketplace_service_item_dropoff_sc * OneFinancialData.quantity;
							allRasxod -= OneFinancialData.item_services.marketplace_service_item_fulfillment * OneFinancialData.quantity;
							allRasxod -= OneFinancialData.item_services.marketplace_service_item_pickup * OneFinancialData.quantity;
							allRasxod -= OneFinancialData.item_services.marketplace_service_item_return_after_deliv_to_customer * OneFinancialData.quantity;
							allRasxod -= OneFinancialData.item_services.marketplace_service_item_return_flow_trans * OneFinancialData.quantity;
							allRasxod -= OneFinancialData.item_services.marketplace_service_item_return_not_deliv_to_customer * OneFinancialData.quantity;
							allRasxod -= OneFinancialData.item_services.marketplace_service_item_return_part_goods_customer * OneFinancialData.quantity;
						}
						else
						{
							string offerId = item.products.ElementAt(item.financial_data.products.IndexOf(OneFinancialData)).offer_id;
							if (offerId.Contains("lnrd"))
							{
								offerId = offerId.Replace("lnrd", "ld");
								offerId = offerId.Replace("_", "-");
							}

							Product oneProduct = AllPrpoduct.Where(x => x.ArticleNumberUnicList.Contains(offerId) || x.ArticleNumberInShop == offerId).FirstOrDefault();
							if (oneProduct != null)
							{
								double kolYmnNaPrice = GetKolYmnNaPrice(offerId, oneProduct.NowPrice, OneFinancialData.quantity, ref countProduct);


								artikylDataBase += oneProduct.ArticleNumberInShop + "\n";
								allZakyyp += kolYmnNaPrice;


								allPrice += OneFinancialData.payout * OneFinancialData.quantity;
								allPrible += OneFinancialData.payout * OneFinancialData.quantity - kolYmnNaPrice;
								allRasxod += kolYmnNaPrice;

								allPrible += OneFinancialData.item_services.marketplace_service_item_deliv_to_customer * OneFinancialData.quantity;
								allPrible += OneFinancialData.item_services.marketplace_service_item_direct_flow_trans * OneFinancialData.quantity;
								allPrible += OneFinancialData.item_services.marketplace_service_item_dropoff_ff * OneFinancialData.quantity;
								allPrible += OneFinancialData.item_services.marketplace_service_item_dropoff_pvz * OneFinancialData.quantity;
								allPrible += OneFinancialData.item_services.marketplace_service_item_dropoff_sc * OneFinancialData.quantity;
								allPrible += OneFinancialData.item_services.marketplace_service_item_fulfillment * OneFinancialData.quantity;
								allPrible += OneFinancialData.item_services.marketplace_service_item_pickup * OneFinancialData.quantity;
								allPrible += OneFinancialData.item_services.marketplace_service_item_return_after_deliv_to_customer * OneFinancialData.quantity;
								allPrible += OneFinancialData.item_services.marketplace_service_item_return_flow_trans * OneFinancialData.quantity;
								allPrible += OneFinancialData.item_services.marketplace_service_item_return_not_deliv_to_customer * OneFinancialData.quantity;
								allPrible += OneFinancialData.item_services.marketplace_service_item_return_part_goods_customer * OneFinancialData.quantity;

								allRasxod -= OneFinancialData.item_services.marketplace_service_item_deliv_to_customer * OneFinancialData.quantity;
								allRasxod -= OneFinancialData.item_services.marketplace_service_item_direct_flow_trans * OneFinancialData.quantity;
								allRasxod -= OneFinancialData.item_services.marketplace_service_item_dropoff_ff * OneFinancialData.quantity;
								allRasxod -= OneFinancialData.item_services.marketplace_service_item_dropoff_pvz * OneFinancialData.quantity;
								allRasxod -= OneFinancialData.item_services.marketplace_service_item_dropoff_sc * OneFinancialData.quantity;
								allRasxod -= OneFinancialData.item_services.marketplace_service_item_fulfillment * OneFinancialData.quantity;
								allRasxod -= OneFinancialData.item_services.marketplace_service_item_pickup * OneFinancialData.quantity;
								allRasxod -= OneFinancialData.item_services.marketplace_service_item_return_after_deliv_to_customer * OneFinancialData.quantity;
								allRasxod -= OneFinancialData.item_services.marketplace_service_item_return_flow_trans * OneFinancialData.quantity;
								allRasxod -= OneFinancialData.item_services.marketplace_service_item_return_not_deliv_to_customer * OneFinancialData.quantity;
								allRasxod -= OneFinancialData.item_services.marketplace_service_item_return_part_goods_customer * OneFinancialData.quantity;
							}
							else
							{
								if (offerId.Contains("Bumaga") || offerId.Contains("kraft_paper"))
								{
									countBumaga++;
									allPriceBumag += OneFinancialData.payout;
								}
								else
								if (offerId.Contains("ld"))
								{
									//string articlelast = offerId.Remove(0, 3);

									string article = "";
									for (int i = 0; i < offerId.Length - 1; i++)
									{
										int ch = 0;
										if (int.TryParse(offerId[i].ToString(), out ch))
										{
											article += ch;
										}
										else
										{
											break;
										}
									}
									Product newPos = LeonardoJobs.AddOneProduct("https://leonardo.ru/ishop/good_" + article, null).Result;
									

									allPrice += OneFinancialData.payout;
									

									allPrible += OneFinancialData.item_services.marketplace_service_item_deliv_to_customer * OneFinancialData.quantity;
									allPrible += OneFinancialData.item_services.marketplace_service_item_direct_flow_trans * OneFinancialData.quantity;
									allPrible += OneFinancialData.item_services.marketplace_service_item_dropoff_ff * OneFinancialData.quantity;
									allPrible += OneFinancialData.item_services.marketplace_service_item_dropoff_pvz * OneFinancialData.quantity;
									allPrible += OneFinancialData.item_services.marketplace_service_item_dropoff_sc * OneFinancialData.quantity;
									allPrible += OneFinancialData.item_services.marketplace_service_item_fulfillment * OneFinancialData.quantity;
									allPrible += OneFinancialData.item_services.marketplace_service_item_pickup * OneFinancialData.quantity;
									allPrible += OneFinancialData.item_services.marketplace_service_item_return_after_deliv_to_customer * OneFinancialData.quantity;
									allPrible += OneFinancialData.item_services.marketplace_service_item_return_flow_trans * OneFinancialData.quantity;
									allPrible += OneFinancialData.item_services.marketplace_service_item_return_not_deliv_to_customer * OneFinancialData.quantity;
									allPrible += OneFinancialData.item_services.marketplace_service_item_return_part_goods_customer * OneFinancialData.quantity;

									allRasxod -= OneFinancialData.item_services.marketplace_service_item_deliv_to_customer * OneFinancialData.quantity;
									allRasxod -= OneFinancialData.item_services.marketplace_service_item_direct_flow_trans * OneFinancialData.quantity;
									allRasxod -= OneFinancialData.item_services.marketplace_service_item_dropoff_ff * OneFinancialData.quantity;
									allRasxod -= OneFinancialData.item_services.marketplace_service_item_dropoff_pvz * OneFinancialData.quantity;
									allRasxod -= OneFinancialData.item_services.marketplace_service_item_dropoff_sc * OneFinancialData.quantity;
									allRasxod -= OneFinancialData.item_services.marketplace_service_item_fulfillment * OneFinancialData.quantity;
									allRasxod -= OneFinancialData.item_services.marketplace_service_item_pickup * OneFinancialData.quantity;
									allRasxod -= OneFinancialData.item_services.marketplace_service_item_return_after_deliv_to_customer * OneFinancialData.quantity;
									allRasxod -= OneFinancialData.item_services.marketplace_service_item_return_flow_trans * OneFinancialData.quantity;
									allRasxod -= OneFinancialData.item_services.marketplace_service_item_return_not_deliv_to_customer * OneFinancialData.quantity;
									allRasxod -= OneFinancialData.item_services.marketplace_service_item_return_part_goods_customer * OneFinancialData.quantity;

									newPos.ArticleNumberUnicList.Clear();
									newPos.ArticleNumberUnicList.Add("ld-" + article + "-x1");
									newPos.ArticleNumberUnicList.Add("ld-" + article + "-x2");
									newPos.ArticleNumberUnicList.Add("ld-" + article + "-x3");
									newPos.ArticleNumberUnicList.Add("ld-" + article + "-x5");
									newPos.ArticleNumberUnicList.Add("ld-" + article + "-x10");

									NewProduct.Add(newPos);
									DataBaseJob.AddListToRemains(NewProduct);
									ProductJobs.parseLeonardoUpdate(newPos);

									double kolYmnNaPrice = GetKolYmnNaPrice(offerId, newPos.NowPrice, OneFinancialData.quantity, ref countProduct);
									allZakyyp += kolYmnNaPrice;
									allPrible += OneFinancialData.payout - kolYmnNaPrice;
									allRasxod += kolYmnNaPrice;


									NewProduct.Clear();

									DataBaseJob.GetAllProductFromTheBase(out Remains, out Wait, out Archive, out Del);

									AllPrpoduct.Clear();
									AllPrpoduct.AddRange(Remains);
									AllPrpoduct.AddRange(Wait);
									AllPrpoduct.AddRange(Archive);
									AllPrpoduct.AddRange(Del);
								}
								else
								{
									countProblem++;
									problem += offerId + "\n";
								}
							}
						}
						
					}

					allPrible += item.financial_data.posting_services.marketplace_service_item_deliv_to_customer;
					allPrible += item.financial_data.posting_services.marketplace_service_item_direct_flow_trans;
					allPrible += item.financial_data.posting_services.marketplace_service_item_dropoff_ff;
					allPrible += item.financial_data.posting_services.marketplace_service_item_dropoff_pvz;
					allPrible += item.financial_data.posting_services.marketplace_service_item_dropoff_sc;
					allPrible += item.financial_data.posting_services.marketplace_service_item_fulfillment;
					allPrible += item.financial_data.posting_services.marketplace_service_item_pickup;
					allPrible += item.financial_data.posting_services.marketplace_service_item_return_after_deliv_to_customer;
					allPrible += item.financial_data.posting_services.marketplace_service_item_return_flow_trans;
					allPrible += item.financial_data.posting_services.marketplace_service_item_return_not_deliv_to_customer;
					allPrible += item.financial_data.posting_services.marketplace_service_item_return_part_goods_customer;

					allRasxod -= item.financial_data.posting_services.marketplace_service_item_deliv_to_customer;
					allRasxod -= item.financial_data.posting_services.marketplace_service_item_direct_flow_trans;
					allRasxod -= item.financial_data.posting_services.marketplace_service_item_dropoff_ff;
					allRasxod -= item.financial_data.posting_services.marketplace_service_item_dropoff_pvz;
					allRasxod -= item.financial_data.posting_services.marketplace_service_item_dropoff_sc;
					allRasxod -= item.financial_data.posting_services.marketplace_service_item_fulfillment;
					allRasxod -= item.financial_data.posting_services.marketplace_service_item_pickup;
					allRasxod -= item.financial_data.posting_services.marketplace_service_item_return_after_deliv_to_customer;
					allRasxod -= item.financial_data.posting_services.marketplace_service_item_return_flow_trans;
					allRasxod -= item.financial_data.posting_services.marketplace_service_item_return_not_deliv_to_customer;
					allRasxod -= item.financial_data.posting_services.marketplace_service_item_return_part_goods_customer;
				}



				if (problem.Length > 0)
					SaveDataInFile(problem, key.Name);

				if (tovarKypl.Length > 0)
					SaveDataInFile(tovarKypl, "Товар " + key.Name);

				if (otpravlNumber.Length > 0)
					SaveDataInFile(otpravlNumber, "Отправления " + key.Name);

				//if (artikylDataBase.Length > 0)
				//	SaveDataInFile(artikylDataBase, "Артикулы " + key.Name);
				

				TextBlock textBlock13 = new TextBlock();
				textBlock13.Text = "Заказано на: " + allSumZak;

				TextBlock textBlock = new TextBlock();
				textBlock.Text = "Всего начислено: " + allPrice;

				TextBlock textBlock2 = new TextBlock();
				textBlock2.Text = "Всего заработано: " + allPrible;

				TextBlock textBlock3 = new TextBlock();
				textBlock3.Text = "Всего потрачено: " + allRasxod;

				TextBlock textBlock14 = new TextBlock();
				textBlock14.Text = "Всего на закуп: " + allZakyyp;
				

				TextBlock textBlock4 = new TextBlock();
				textBlock4.Text = "Всего продуктов: " + countProduct;
				TextBlock textBlock12 = new TextBlock();
				textBlock12.Text = "Всего озоновских карточек: " + countProductOzon;
				
				TextBlock textBlock11 = new TextBlock();
				textBlock11.Text = "Всего отправлений: " + countOtpravl;
				
				TextBlock textBlock10 = new TextBlock();
				textBlock10.Text = "Возвращено: " + countVozvrat;

				TextBlock textBlock7 = new TextBlock();
				textBlock7.Text = "Бумага: " + countBumaga;

				TextBlock textBlock8 = new TextBlock();
				textBlock8.Text = "Получено на бумаге: " + allPriceBumag;

				TextBlock textBlock5 = new TextBlock();
				textBlock5.Text = "Всего проблемных: " + countProblem;

				TextBlock textBlock6 = new TextBlock();
				textBlock6.Text = key.Name + ": ";

				stackPanel.Children.Add(textBlock6);
				stackPanel.Children.Add(textBlock13);
				stackPanel.Children.Add(textBlock);
				stackPanel.Children.Add(textBlock2);
				stackPanel.Children.Add(textBlock3);
				stackPanel.Children.Add(textBlock14);
				stackPanel.Children.Add(textBlock4);
				stackPanel.Children.Add(textBlock12);
				stackPanel.Children.Add(textBlock11);
				stackPanel.Children.Add(textBlock10);
				stackPanel.Children.Add(textBlock5);
				stackPanel.Children.Add(textBlock7);
				stackPanel.Children.Add(textBlock8);

				if (NewProduct.Count > 0)
				{
					DataBaseJob.AddListToRemains(NewProduct);
					TextBlock textBlock9 = new TextBlock();
					textBlock9.Text = "Новое в бд: " + NewProduct.Count;
					stackPanel.Children.Add(textBlock9);
				}
			}
			TestFinansyMainGrid.Children.Add(stackPanel);
		}
		private async void SaveDataInFile(string data, string nameFile)
		{
			FolderPicker folderPicker = new FolderPicker();
			folderPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
			folderPicker.FileTypeFilter.Add("*");
			StorageFolder fileWithLinks = await folderPicker.PickSingleFolderAsync();
			if (fileWithLinks != null)
			{
				await fileWithLinks.CreateFileAsync(nameFile + ".txt", CreationCollisionOption.ReplaceExisting);
				StorageFile myFile = await fileWithLinks.GetFileAsync(nameFile + ".txt");
				await FileIO.WriteTextAsync(myFile, data);
			}
		}
	}
}
