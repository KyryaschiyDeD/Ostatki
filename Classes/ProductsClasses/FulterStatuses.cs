using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Остатки.Classes.ProductsClasses
{
	public class FulterStatuses
	{
        public static List<string> FilterList = new List<string>()
        {
            //"VISIBLE", // видимый
            "INVISIBLE", // невидимый
            //"EMPTY_STOCK", // ПУСТОЙ ЗАПАС
            //"NOT_MODERATED", // НЕ МОДЕРИРУЕТСЯ
            //"MODERATED", // модерируемый
            //"DISABLED", // отключенный
            //"STATE_FAILED", // СОСТОЯНИЕ ЗАВЕРШЕНО
            //"READY_TO_SUPPLY", // ГОТОВНОСТЬ К ПОСТАВКЕ
            //"EMPTY_NAVIGATION_CATEGORY", // ПУСТАЯ КАТЕГОРИЯ НАВИГАЦИИ
            //"VALIDATION_STATE_PENDING", // СОСТОЯНИЕ ПРОВЕРКИ В ОЖИДАНИИ
            //"VALIDATION_STATE_FAIL", // СБОЙ СОСТОЯНИЯ ПРОВЕРКИ
            //"VALIDATION_STATE_SUCCESS",// СОСТОЯНИЕ ПРОВЕРКИ УСПЕШНО
            //"TO_SUPPLY", // Поставлять
            //"IN_SALE", // В продаже
            //"REMOVED_FROM_SALE", // Удалено из продаже
            //"ARCHIVED", // В архиве
        };

        public static  List<string> FilterListRU = new List<string>()
        {
            "Видимый",
            "Невидимый",
            "Пустой запас",
            "Не модерируется",
            "Модерируемый",
            "Отключенный",
            "Состояние завершено",
            "Готовность к поставке",
            "Пустая категория навигации",
            "Состояние проверки в ожидании",
            "Сбой состояния проверки",
            "Состояние проверки успешно",
            "Поставлять",
            "В продаже",
            "Удалено из продаже", 
            "В архиве"
        };
    }
}
