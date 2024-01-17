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
        [HttpGet]
        public IActionResult GetJsonData()
        {
            SomokatContext context = new();
            var data = context.ChargingStations;

            var json = JsonConvert.SerializeObject(data);
            JArray jsonArray = JArray.Parse(json);
            JArray features = new JArray();

            int idCounter = 0;
            foreach (JObject obj in jsonArray)
            {
                double x = (double)obj["Location"]["X"];
                double y = (double)obj["Location"]["Y"];

                JObject geometry = new JObject(
                    new JProperty("type", "Point"),
                    new JProperty("coordinates", new JArray { x, y })
                );

                JObject properties = new JObject(
                    new JProperty("balloonContent", "Содержимое балуна")
                );

                JObject feature = new JObject(
                    new JProperty("type", "Feature"),
                    new JProperty("id", idCounter),
                    new JProperty("geometry", geometry),
                    new JProperty("properties", properties)
                );

                features.Add(feature);
                idCounter++;
            }

            JObject featureCollection = new JObject(
                new JProperty("type", "FeatureCollection"),
                new JProperty("features", features)
            );

            string json2 = featureCollection.ToString();
            return Content(json2, "application/json");
        }
    }
}

