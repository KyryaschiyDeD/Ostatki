using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Остатки.Classes;
using Остатки.Classes.Taxation.TaxClasses;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace Остатки.Pages.Taxation
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class StatisticTaxation : Page
    {
        public StatisticTaxation()
        {
            this.InitializeComponent();
            List<ReceiptOnDB> tmpTaxBD = new List<ReceiptOnDB>();
            using (var db = new LiteDatabase($@"{Global.folder.Path}/TaxsDB.db"))
            {
                var col = db.GetCollection<ReceiptOnDB>("Receipts");
                tmpTaxBD = new List<ReceiptOnDB>(col.Query().ToList());
            }
            double sum = 0;

            foreach (var oneTaxDB in tmpTaxBD)
            {
                foreach (var oneReceiptItem in oneTaxDB.receipt.items)
                {
                    if (oneReceiptItem.IsFound)
                        sum += oneReceiptItem.sum;
                    else
                    if (oneReceiptItem.quantity != oneReceiptItem.remainsQuantity)
                    {
                        if (oneReceiptItem.remainsQuantity < 0)
                            oneReceiptItem.remainsQuantity = 0;
                        sum += oneReceiptItem.price * (oneReceiptItem.quantity - oneReceiptItem.remainsQuantity);

                    }
                }
            }
            TextNalogRasxod.Text = (sum/100).ToString();
        }
    }
}
