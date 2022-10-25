using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Остатки.Classes.Taxation.TaxClasses;

namespace Остатки.Classes.Taxation
{
    public class TaxationJobClass
    {
        private static readonly string apiToken = "15239.20dUQQYmlHxbOPLzb";
        private static readonly Proverkacheka proverkacheka = new Proverkacheka(apiToken);

        public static async void GetAndSaveTax (string code)
        {
            List<ReceiptOnDB> tmpTaxBD = new List<ReceiptOnDB>();
            using (var db = new LiteDatabase($@"{Global.folder.Path}/TaxsDB.db"))
            {
                var col = db.GetCollection<ReceiptOnDB>("Receipts");
                tmpTaxBD = new List<ReceiptOnDB>(col.Query().ToList());
            }

            if (tmpTaxBD.Find(x => x.ReceivingLine == code.Replace(" ","")) == null)
            {
                Thread.Sleep(750);
                Receipt newReceipt = await GetReciept(code);
                if (newReceipt.items == null || newReceipt.items.Count == 0)
                {
                    Thread.Sleep(1000);
                    newReceipt = await GetReciept(code);
                }
                /* if (newReceipt.items == null || newReceipt.items.Count == 0)
                 {
                     Thread.Sleep(1000);
                     newReceipt = await GetReciept(code);
                 }
                 if (newReceipt.items == null || newReceipt.items.Count == 0)
                 {
                     Thread.Sleep(1000);
                     newReceipt = await GetReciept(code);
                 }*/
                int ind = 0;

                Guid guid = Guid.NewGuid();

                foreach (var oneProduct in newReceipt.items)
                {
                    oneProduct.Id = ++ind;
                    oneProduct.dateBuy = newReceipt.dateTime;
                    oneProduct.ReceiptOnDBID = guid;
                    oneProduct.remainsQuantity = oneProduct.quantity;
                }

                var folder = Global.folder;
                using (var db = new LiteDatabase($@"{folder.Path}/TaxsDB.db"))
                {
                    var col = db.GetCollection<ReceiptOnDB>("Receipts");
                    var proverk = col.FindOne(x => x.receipt.fiscalDriveNumber == newReceipt.fiscalDriveNumber && x.receipt.fiscalDocumentNumber == newReceipt.fiscalDocumentNumber && x.receipt.fiscalSign == newReceipt.fiscalSign);
                    if (proverk == null)
                        col.Insert(
                            new ReceiptOnDB()
                            {
                                ID = guid,
                                receipt = newReceipt,
                                DateAddOnDB = DateTime.Now,
                                ReceivingLine = code,
                                CountPosFinding = 0,
                            }
                            );
                }
            }
        }

        private static async Task<Receipt> GetReciept(string qrRaw)
        {
            return await proverkacheka.GetAsyncByRaw(qrRaw);
        }
    }
}
