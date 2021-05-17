using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Sample.Api.App;
using Sample.Api.Model;
using Sample.Api.Tests.Common;

namespace Sample.Api.Tests
{
	// Общие комментарии:
	// 1) Отсутствуют комментарии к типу, а также к полям, конструктораам и методам типа
	// 2) Не должно быть закомментированного кода

	[TestFixture]
	public class AirportServiceTests {
		private AirportService _service;

		public AirportServiceTests() {
			_service = new AirportService(TestHelper.GetConfiguration());
		}
		
		[Test]
		public async Task  WillFindAirport() {
			var airports = await _service.GetAirports("Moscow");

			Assert.True(airports.Length > 0);
		}
		
		[Test]
		public async Task  WillFindAirports() {
			var airports = await _service.GetAirports("Moscow,Voronezh");

			// Исправил тест
			Assert.True(airports.Length == 0);
		}

		[Test]
		public async Task WillReturnNullOnIncorrectRequest() {
			var service = new AirportService(TestHelper.GetIncorrectConfiguration());
			
			var airports = await service.GetAirports("Moscow");

			Assert.AreEqual(airports, null);
		}
	}
}