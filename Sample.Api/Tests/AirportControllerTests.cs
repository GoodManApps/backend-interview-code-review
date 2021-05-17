using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Sample.Api.App;
using Sample.Api.Controllers;
using Sample.Api.Model;
using Sample.Api.Tests.Common;

namespace Sample.Api.Tests
{
	// Общие комментарии:
	// 1) Отсутствуют комментарии к типу, а также к полям, конструктораам и методам типа
	// 2) Не должно быть закомментированного кода

	[TestFixture]
	public class AirportControllerTests {
		private readonly AirportController _controller;

		public AirportControllerTests() {
			var service = new AirportService(TestHelper.GetConfiguration());
			_controller = new AirportController(service);
		}
		
		[Test]
		public void WillFindShortestAirportDistance() {
			IActionResult result = _controller.FindNearestAsync(new[] { "Moscow", "Saint-Petersburg" }).Result;

			Assert.IsInstanceOf<OkObjectResult>(result);
			var pair = ((List<AirportPair>) ((OkObjectResult) result).Value).Single();
			Assert.That(pair.First.City, Is.EqualTo("Moscow"));
			Assert.That(pair.Second.City, Is.EqualTo("Saint-Petersburg"));
		}
		
		[Test]
		public void WillReturnNoAirportPairErrorForFirstCity() {
			IActionResult result = _controller.FindNearestAsync(new[] { "test", "Saint-Petersburg" }).Result;

			Assert.IsInstanceOf<OkObjectResult>(result);
			Assert.That(((OkObjectResult)result).Value, Is.EqualTo("Can't build pair of cities"));
		}

		[Test]
		public void WillReturnNoAirportErrorForFirstCity() {
			IActionResult result = _controller.FindNearestAsync(new[] { "test", "sdfs" }).Result;

			Assert.IsInstanceOf<NotFoundObjectResult>(result);
			Assert.That(((NotFoundObjectResult)result).Value, Is.EqualTo("There are no airports found for cities"));
		}
	}
}
