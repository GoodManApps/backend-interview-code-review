using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sample.Api.App;
using Sample.Api.Model;

namespace Sample.Api.Controllers
{
	// 1) Вобавить с помощью DI экземпляр сервиса IAiportService, присвоив корерктное имя
	// 2) Заменить метод Get на Post для правильной реализации RestAPI, получая данные из тела запроса, а также изменив имя в lowercase
	// 3) Вместо использования try..catch внутри каждого метода, можно добавить глобальный обработчики исключений

	[Route("api/[controller]")]
	[ApiController]
	public class AirportController : ControllerBase
	{
		private readonly IAiportService _aiportService;

		public AirportController(IAiportService aiportService)
		{
			_aiportService = aiportService;
		}

		/// Will find closest airports route for every city pair
		[HttpPost("nearest-airport")]
		public async Task<IActionResult> FindNearestAsync([FromBody] string[] cities)
		{
			try
			{
				// Понятное название переменной в двух местах
				var airports = await _aiportService.GetAirportsAsync(cities);

				// Проверим, что найдены аэропорты
				if (!airports.Any())
					// Заменить на NotFound
					return NotFound("There are no airports found for cities");

				List<AirportPair> shortest = new List<AirportPair>();
				var pairsDistance = _aiportService
					.CalculateDistance(airports)
					.GroupBy(_ => _.Distance);

				// Проверка, что сформированы правильно пары
				if (!pairsDistance.Any())
					return Ok("Can't build pair of cities");

				// Найдём минимально расстояние
				var minDistance = pairsDistance.Min(_ => _.Key);

				var shortestAirports = pairsDistance
					.Where(p => p.Key <= minDistance);

				foreach (var distanceGroup in shortestAirports)
				{
					if (distanceGroup.Count() != 2)
					{
						throw new ApplicationException("Pair of cities was build incorrectly");
					}

					if (distanceGroup.ElementAt(0).IsSameRoute(distanceGroup.ElementAt(1)))
					{
						shortest.Add(distanceGroup.ElementAt(0));
					}
				}

				return Ok(shortest);
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
		}
	}
}
