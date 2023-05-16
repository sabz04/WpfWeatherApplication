using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfWeatherApplication.HttpRequestsHelper
{
    /// <summary>
    /// Класс конфигурации запросов к Апишке.
    /// </summary>
    public static class ApiRoutes
    {
        public static string TOKEN = "571e17255dda9c4e84141179e6ff8635";

        /// <summary>
        /// Домен.
        /// </summary>
        public static string MAIN_HOST = "http://api.openweathermap.org";
        /// <summary>
        /// Статический метод, принимающий в качестве аргумента название города - поисковый запрос. Возвращает сформированную строку запроса к Api для получения координат города.
        /// </summary>
        /// <param name="cityName"></param>
        /// <returns></returns>
        public static string GetGeoRoute(string cityName)
        {
            return $"{MAIN_HOST}/geo/1.0/direct?q={cityName}&lang=ru&limit=5&appid={TOKEN}";
        }
        /// <summary>
        /// Статический методы, принимающий в качестве параметров долготу и широту(координаты города). Возвращает сформированную строку запроса к Api для получения погоды.
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <returns></returns>
        public static string GetWeatherRoute(double lat, double lon)
        { 
            return $"{MAIN_HOST}/data/2.5/weather?lat={lat}&lon={lon}&lang=ru&appid={TOKEN}&units=metric ";
        }

    }
}
