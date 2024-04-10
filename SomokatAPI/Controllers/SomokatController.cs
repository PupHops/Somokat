using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Somokat;
using System.Threading.Tasks;
using System.Linq;

namespace SomokatAPI.Controllers
{



    public class ScooterRent
    {
        public int userId {  get; set; }
        public int targetScooter { get; set; }

    }

  

    [ApiController]
    [Route("[controller]")]
    public class SomokatController : Controller
    {
      
       

      
      


        [HttpPost("Rent")]
        public async Task<IActionResult> Rent([FromBody] ScooterRent requestBody)
        {
            var dbContext = new SomokatContext();

            int idScooter = Convert.ToInt32(requestBody.targetScooter);
            
            var scooter = await dbContext.Scooters.FindAsync(idScooter);

            if (scooter == null)
            {
                return NotFound();
            }

            scooter.IdStatus = 4; // Устанавливаем статус "Арендован"

            await dbContext.SaveChangesAsync();
            Task.Run(() => DeductBalancePeriodically(requestBody));


            return NoContent();
        }

        private async Task DeductBalancePeriodically(ScooterRent requestBody)
        {
           

            while (true)
            {
                await Task.Delay(TimeSpan.FromMinutes(0.25)); // Ждем 1 минуту
                SomokatContext _context = new SomokatContext();

                var user = await _context.UserAccounts.FindAsync(requestBody.userId);
                // Проверяем, что самокат все еще арендуется
                var scooter = await _context.Scooters.FindAsync(requestBody.targetScooter);
                if (scooter.IdStatus != 4)
                {
                    return;
                }

                user.Bonus -= 1; // Уменьшаем баланс пользователя
                await _context.SaveChangesAsync();
            }
        }

        [HttpPost("EndTrip")]
        public async Task<IActionResult> EndTrip([FromBody] ScooterRent requestBody)
        {
            var _context = new SomokatContext();

            var scooter = await _context.Scooters.FindAsync(requestBody.targetScooter);

            if (scooter == null)
            {
                return NotFound();
            }

            scooter.IdStatus = 1; // Устанавливаем статус "Доступен"

            await _context.SaveChangesAsync();
            return NoContent();
        }


        [HttpGet]
        public IActionResult GetJsonData()
        {
            string jsonResult = null;
            using (var dbContext = new SomokatContext())
            {
                // Получение списка скутеров из базы данных
                List<Scooter> scooters = dbContext.Scooters.Where(s => s.IdStatus == 1).ToList();

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
                            balloonContent = $"{scooter.BatteryLevel};{scooter.IdStatus};{scooter.Id}",
                            clusterCaption = "",
                            hintContent = $""
                        }
                    }).ToList()
                };

                return JsonConvert.SerializeObject(featureCollection);
            }

            return Content(jsonResult, "application/json");
        }
    }


}

