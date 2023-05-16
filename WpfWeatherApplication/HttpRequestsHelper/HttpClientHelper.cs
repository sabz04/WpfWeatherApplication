using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WpfWeatherApplication.HttpRequestsHelper
{
    /// <summary>
    /// Класс для отправки запросов - получения ответа. Взаимодействия с запросами реализованы через класс HttpClient.
    /// </summary>
    public class HttpClientHelper
    {
        private readonly HttpClient _httpClient;

        public HttpClientHelper()
        {
            _httpClient = new HttpClient();
        }
        /// <summary>
        /// Асинхронная операция, принимает параметр ссылки для отправки запроса.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<string> GetAsync(string url)
        {
            var httpResponse = await _httpClient.GetAsync(url);
            if (!httpResponse.IsSuccessStatusCode)
            {
                return "";
            }
            var responseString = await httpResponse.Content.ReadAsStringAsync();
            return responseString;
        }
    }
}
