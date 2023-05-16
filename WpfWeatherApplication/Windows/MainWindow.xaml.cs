using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfWeatherApplication.HttpRequestsHelper;
using WpfWeatherApplication.Models;

namespace WpfWeatherApplication
{
    /// <summary>
    /// Логика взаимодействия с MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {

        private HttpClientHelper _httpRequestsHelper;
        private readonly static string NONE_STRING = "Неизвестно";
        /// <summary>
        /// Конструктор, создает объект класса для взаимодействия с апишкой.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            _httpRequestsHelper= new HttpClientHelper();
        }
        /// <summary>
        /// Обработчик событий кнопки для отправки HTTP-запросов к апишке погоды.
        /// Для удобного взаимодействия с апишкой создал специальный класс, 
        /// реализующий функционал тасков для реализации возможностей асинхронных операций.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void searchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string currentSearch = searchTextBox.Text;
                //для удобства и эстетики заранее заполнил значения будущих полей на вывод.
                string description = NONE_STRING;
                string cityName = NONE_STRING;
                string temp = NONE_STRING;
                string tempMax = NONE_STRING;
                string tempMin = NONE_STRING;
                string countryName = NONE_STRING;
                string windSpeed = NONE_STRING;

                if (String.IsNullOrEmpty(currentSearch))
                {
                    MessageBox.Show("Пустая строка.");
                    return;
                }
                //запрос на получение координат по поиску
                var findedCityCoordinates = await GetCityCoordinates(_httpRequestsHelper, currentSearch);

                if (findedCityCoordinates == null)
                {
                    return;
                }
                //запрос на получение погоды по координатам
                var findedCityWeather = await GetCityWeather(_httpRequestsHelper, findedCityCoordinates.lat, findedCityCoordinates.lon);

                if (findedCityWeather == null)
                {
                    return;
                }
                //проверки на значения null и пустые строки, чтобы программа не крашилась и выводила корректный результат,
                //соответствующий дружественному интерфейсу, заметил, что часто попадаются пустые и нулевые значения.
                if (findedCityCoordinates.local_names != null && findedCityCoordinates.local_names.ru != null)
                    cityName = findedCityCoordinates.local_names.ru;
                else if (findedCityCoordinates.name != string.Empty)
                    cityName = findedCityCoordinates.name;
                if (findedCityCoordinates.country != null && findedCityCoordinates.country !=  string.Empty)
                    countryName = findedCityCoordinates.country;
                if (findedCityWeather.main != null)
                {
                    temp = findedCityWeather.main.temp.ToString();

                    tempMax = findedCityWeather.main.temp_max.ToString();

                    tempMin = findedCityWeather.main.temp_min.ToString();
                }
                if (findedCityWeather.wind != null)
                {
                    windSpeed = findedCityWeather.wind.speed.ToString();
                }
                if (findedCityWeather.weather.Length > 0)
                {
                    description = findedCityWeather.weather[0].description;
                }

                var weatherDescText =
                    $"Город: {cityName}, Страна: {countryName}" +
                    $"\nТемпература сейчас: {temp}, Минимум: {tempMin}, Максимум: {tempMax}" +
                    $"\nСкорость ветра: {windSpeed}" +
                    $"\nСостояние погоды: {description}";
                weatherDescriptionTextBlock.Text = weatherDescText;
            }
           catch(Exception ex)
            {
                MessageBox.Show($"Ошибка. Подробнее: {ex.Message}");
            }
        }
        /// <summary>
        /// Асинхронный метод получения координат населенного пункта по введенному поиску, принимает параметр поиска в качестве названия города и отправляет запрос на получение данных.
        /// Сделал метод независимыми от внешних элементов, некая простая реализация инверсии зависимостей.
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        private async Task<CityGeoObject> GetCityCoordinates(HttpClientHelper httpClientHelper, string search)
        {
            try
            {
                string response = await httpClientHelper.GetAsync(ApiRoutes.GetGeoRoute(search));

                var cityCoordinates = JsonSerializer.Deserialize<CityGeoObject[]>(response);

                if (cityCoordinates == null || cityCoordinates.Length < 1)
                {
                    MessageBox.Show("Ничего не найдено!");
                    return null;
                }

                return cityCoordinates[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка. Подробнее: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Окончательный метод получения погоды по координатам.
        /// Принимает долготу и широту - координаты, которые были получены ранее. Обращение идет к тому же домену, что и для получения координат.
        /// Сделал метод независимыми от внешних элементов, некая простая реализация инверсии зависимостей.
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <returns></returns>
        private async Task<CityWeatherRootObject> GetCityWeather(HttpClientHelper httpClientHelper ,double lat, double lon)
        {
            try
            {
                string response = await httpClientHelper.GetAsync(ApiRoutes.GetWeatherRoute(lat, lon));

                var cityWeather = JsonSerializer.Deserialize<CityWeatherRootObject>(response);

                if (cityWeather == null)
                {
                    MessageBox.Show("Ничего не найдено!");
                    return null;
                }

                return cityWeather;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка. Подробнее: {ex.Message}");
                return null;
            }
        }
    }
}
