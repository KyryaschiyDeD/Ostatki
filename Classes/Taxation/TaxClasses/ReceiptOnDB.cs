using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.Taxation.TaxClasses
{
    public class ReceiptOnDB
    {
        public Guid ID { get; set; }

        /// <summary>
        /// Чек
        /// </summary>
        public Receipt receipt { get; set; }

        /// <summary>
        /// Дата добавления в бд
        /// </summary>
        public DateTime DateAddOnDB { get; set; }

        /// <summary>
        /// Строка получения чека (QR код с чека)
        /// </summary>
        public string ReceivingLine { get; set; }

        /// <summary>
        /// Количество найденных отправлений в чеке
        /// </summary>
        public int CountPosFinding { get; set; }

        /// <summary>
        /// Бумажный чек или электронный
        /// true - бумага
        /// flase - электро
        /// </summary>
        public bool PaperOrElectronic { get; set; }

    }
}
