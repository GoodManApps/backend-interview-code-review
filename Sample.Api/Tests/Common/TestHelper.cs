using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.Api.Tests.Common
{
    /// <summary>
    /// Вспомогательный тип для тестов
    /// </summary>
    public static class TestHelper
    {
        /// <summary>
        /// Получить корретно настроенный конфигурационный файл
        /// </summary>
        /// <returns></returns>
        public static IConfiguration GetConfiguration()
        {
            var myConfiguration = new Dictionary<string, string>
            {
                {"Endpoints:AirportService", "https://homework.appulate.dev/api/" }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();

            return configuration;
        }

        /// <summary>
        /// Получить некорретно настроенный конфигурационный файл
        /// </summary>
        /// <returns></returns>
        public static IConfiguration GetIncorrectConfiguration()
        {
            var myConfiguration = new Dictionary<string, string>
            {
                {"Endpoints:AirportService", "https://example.org"}
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();

            return configuration;
        }
    }
}
