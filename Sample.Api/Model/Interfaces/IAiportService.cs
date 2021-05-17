using Sample.Api.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample.Api.App
{
    /// <summary>
    /// Сервис получения информации по аэропортам
    /// </summary>
    public interface IAiportService: IDisposable
    {
        /// <summary>
        /// Рассчитать дистанцию между аэропортами
        /// </summary>
        /// <param name="airports">Список аропортов</param>
        /// <returns></returns>
        IEnumerable<AirportPair> CalculateDistance(IEnumerable<Airport> airports);

        /// <summary>
        /// Получить список аэропортов
        /// </summary>
        /// <param name="cities">Список городов, по которым необходимо получить список аэропортов</param>
        /// <returns></returns>
        Task<IEnumerable<Airport>> GetAirportsAsync(string[] cities);
    }
}