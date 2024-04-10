using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Somokat;

namespace SomokatAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SomokatController : Controller
    {

        [HttpPost("Rent")]
        public IActionResult RentScooter(int id)
        {
            return null;
        }

        [HttpGet]
        public IActionResult GetJsonData()
        {
            string jsonResult = null;
            using (var dbContext = new SomokatContext())
            {
                // Получение списка скутеров из базы данных
                List<Scooter> scooters = dbContext.Scooters.ToList();

                // Преобразование в JSON формат
                jsonResult = ConvertToGeoJson(scooters);
            }
            static string ConvertToGeoJson(List<Scooter> scooters)
            {
                var featureCollection = new
                {
                    type = "FeatureCollection",
                    features = scooters.Select(scooter => new
                    {
                        type = "Feature",
                        id = scooter.Id,
                        geometry = new
                        {
                            type = "Point",
                            coordinates = new[] { scooter.Location.X, scooter.Location.Y }
                        },
                        properties = new
                        {
                            balloonContent = $"{scooter.BatteryLevel};{scooter.IdStatus}",
                            clusterCaption = "",
                            hintContent = ""
                        }
                    }).ToList()
                };

                return JsonConvert.SerializeObject(featureCollection);
            }

            return Content(jsonResult, "application/json");
        }
    }


}

