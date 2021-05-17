using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Sample.Api.Model;

namespace Sample.Api.App
{
	// Общие комментарии:
	// 1) Отсутствуют комментарии к типу, а также к полям, конструктораам и методам типа
	// 2) Сервис не должен получать адрес api напрямую в конструкторе. 
	// Небходимо создать интерфейс, который должен реализовать текущий сервис (решает несколько задач, в том числе использование Mock в тестах).
	// В качестве параметра конструктора выполнить передачу настроек конфигурации из appsettings
	// 2.1) Желательно не наследоваться от HttpClient, а создать скрытый экземпляр этого типа. И в методах делать запросы с его помощью,
	// а также реализовать интерфейс IDisposable, где бы освобожались ресурсы клиента.
	// 3) Не должно быть закомментированного кода
	// 4) Возвращаемые перечисляемые типы желательно объявлять IEnumerable
	// 5) Нет валидации параметров (для примера добавил в конструктор

	public class AirportService : IAiportService
	{
		// Переменная должна иметь модификатор доступа private readonly
		private readonly string ApiHost;
		private readonly HttpClient client;
		private readonly DistanceCalculator distanceHelper;
		private bool disposedValue;
		private const string aiportEndpoint = "Endpoints:AirportService";
		private const string aiportSerachUrl = "Airport/search";

		public AirportService(IConfiguration configuration)
		{
			if (configuration == null)
				throw new ArgumentNullException(nameof(configuration));

			ApiHost = configuration.GetSection(aiportEndpoint)?.Value;

			if (!Uri.TryCreate(ApiHost, UriKind.Absolute, out Uri BaseUri))
				throw new ArgumentException("Некорректный формат адреса URL в файле конфигураций", nameof(configuration));

			client = new HttpClient()
			{
				BaseAddress = BaseUri
			};

			distanceHelper = new DistanceCalculator();
		}

		public IEnumerable<AirportPair> CalculateDistance(IEnumerable<Airport> airports)
		{
			// distanceHelper перенёс в область члена типа
			// Вместо бессмысленного двойного перебора по очереди использовать код:
			var airportsPairs = airports.
				SelectMany(currentPort =>
				{
					return airports
						.Where(airport => !airport.City.Equals(currentPort.City))
						.Select(airport =>
						{
							return new AirportPair()
							{
								First = currentPort,
								Second = airport,
								Distance = distanceHelper.DistanceBetweenPlaces(currentPort, airport)
							};
						});					
				});

			return airportsPairs;
		}

		public async Task<IEnumerable<Airport>> GetAirportsAsync(string[] cities)
		{
			// Преобразовать код вместо перебора на LINQ запрос, распаралеллив запросы по описку:

			var tasks = await Task.WhenAll(
				cities
					.Select(city => city.ToLower())
					.Distinct()
					.Select(async city =>
					{
						return await GetAirports(city);
					}));

			var ports = tasks
				.Where(result => result != null)
				.SelectMany(_ => _.Select(airport => airport));

			return ports;
		}

		// Переименовать метод, так как возвращается массив
		public async Task<Airport[]> GetAirports(string city)
		{
			// Перенёс переменную в константы типы.
			
			Uri requestUri = new Uri($"{aiportSerachUrl}?pattern={city}", UriKind.Relative);
			try {
				var response = await client.GetAsync(requestUri);

				Airport[] result = null;
				if (response.IsSuccessStatusCode) {
					// Для более быстрого парсинга стоит использовать альтерный json converter, например, Utf8Json
					result = await response.Content.ReadAsAsync<Airport[]>();
					return result;
				} else {
					return null;
				}
			} catch (Exception ex) {
				// Стоит использовать логгер для фиксации таких ошибок, например, NLog
				System.Diagnostics.Trace.TraceError($"Something happen during web service call: {ex.Message}");
				throw ex;
			}

			// Удалил недостижимый код
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					client.Dispose();
				}

				// TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить метод завершения
				// TODO: установить значение NULL для больших полей
				disposedValue = true;
			}
		}

		public void Dispose()
		{
			// Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}