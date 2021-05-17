namespace Sample.Api.Model
{
	// Общие комментарии:
	// 1) Отсутствуют комментарии к типу, а также к полям, конструктораам и методам типа
	// 2 Отсутствует поле alias, country и altitude для аэропорта
	// 3) Заменил бы структура на тип для аэропорта

	public class Airport {
		public string Name { get; set; }
		public string City { get; set; }
		public double Latitude { get; set; }
		public double Longitude { get; set; }
	}

	public class AirportPair {
		public Airport First { get; set; }
		public Airport Second { get; set; }
		public double Distance { get; set; }
		public override string ToString() {
			return $"{First.City}:{First.Name} to {Second.City}:{Second.Name} = {Distance.ToString("N2")}";
		}

		public bool IsSameRoute(AirportPair pair) {
			return (First.City == pair.First.City && Second.City == pair.Second.City) || (First.City == pair.Second.City && Second.City == pair.First.City);
		}
	}
}