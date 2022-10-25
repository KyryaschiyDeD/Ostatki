using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.Taxation.TaxClasses
{
    public class Receipt
    {
        public int code { get; set; }

        /// <summary>
        /// Огранизация
        /// </summary>
        public string user { get; set; }

        /// <summary>
        /// Товары
        /// </summary>
        public List<Item> items { get; set; }
        public int nds18 { get; set; }

        /// <summary>
        /// Регион
        /// </summary>
        public string region { get; set; }

        public string userInn { get; set; }

        /// <summary>
        /// Время
        /// </summary>
        public DateTime dateTime { get; set; }
        public string kktRegId { get; set; }
        public Metadata metadata { get; set; }
        public string @operator { get; set; }

        /// <summary>
        /// Итого (сумма в коп.)
        /// </summary>
        public double totalSum { get; set; }

        /// <summary>
        /// Сумма постоплаты (кредитами) 
        /// </summary>
        public double creditSum { get; set; }

        public string numberKkt { get; set; }

        public long fiscalSign { get; set; }

        /// <summary>
        /// Сумма предоплаты (авансами)
        /// </summary>
        public double prepaidSum { get; set; }

        /// <summary>
        /// Место чека
        /// </summary>
        public string retailPlace { get; set; }

        /// <summary>
        /// Номер смены 
        /// </summary>
        public int shiftNumber { get; set; }

        /// <summary>
        /// Наличные (сумма в коп.)
        /// </summary>
        public double cashTotalSum { get; set; }

        /// <summary>
        /// Сумма по встречным
        /// предоставлениями
        /// </summary>
        public double provisionSum { get; set; }

        /// <summary>
        /// Картой (сумма в коп.)
        /// </summary>
        public double ecashTotalSum { get; set; }

        /// <summary>
        /// Тип операции:
        /// 1 - Приход
        /// 2 - Возврат прихода
        /// 3 - Расход
        /// 4 - Возврат расхода
        /// </summary>
        public int operationType { get; set; }

        /// <summary>
        /// Номер чека за смену
        /// </summary>
        public int redefine_mask { get; set; }

        public int requestNumber { get; set; }

        /// <summary>
        /// Заводской номер фискального
        /// накопителя
        /// </summary>
        public string fiscalDriveNumber { get; set; }

        public double messageFiscalSign { get; set; }

        /// <summary>
        /// Применяемая система
        /// налогообложения:
        /// • 1 – ОСН
        /// • 2 – УСН доход
        /// • 4 – УСН доход - Расход
        /// • 8 – ЕНВД
        /// • 16 – ЕСХН
        /// • 32 – Патент
        /// </summary>
        public int appliedTaxationType { get; set; }

        /// <summary>
        /// Порядковый номер фискального
        /// документа
        /// </summary>
        public int fiscalDocumentNumber { get; set; }

        /// <summary>
        /// Версия формата фискальных
        /// данных:
        /// • 1 – версия ФФД 1.0
        /// • 2 – версия ФФД 1.05
        /// • 3 – версия ФФД 1.1
        /// </summary>
        public int fiscalDocumentFormatVer { get; set; }

        /// <summary>
        /// Сообщение, если не удалась добавить чек
        /// </summary>
        public string Message { get; } = "";

        public Receipt() { }
        public Receipt(string message) => Message = message;
    }
}
