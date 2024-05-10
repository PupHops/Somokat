using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Somokat;
using System.Threading.Tasks;
using System.Linq;
using System.Drawing;
using NpgsqlTypes;
using System;
using Npgsql;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
namespace SomokatAPI.Controllers
{



    public class ScooterRent
    {
        public int userId { get; set; }
        public int targetScooter { get; set; }

    }
    public class UserPay
    {
        public int userId { get; set; }
        public int ValuePay { get; set; }

    }





    [ApiController]
    [Route("[controller]")]
    public class SomokatController : Controller
    {
 

        [HttpPost("NearestScooter")]
        public async Task<IActionResult> FindNearestScooter(double x, double y)
        {
            var dbContext = new SomokatContext();

            var users = dbContext.Scooters.FromSqlRaw($"SELECT * FROM scooter ORDER BY location <-> point({x}, {y}) LIMIT 1");

            return StatusCode(200, new { users});
        }



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

            scooter.IdStatus = 4; 

            await dbContext.SaveChangesAsync();
            Task.Run(() => DeductBalancePeriodically(requestBody));


            return NoContent();
        }

        private async Task DeductBalancePeriodically(ScooterRent requestBody)
        {


            while (true)
            {
                await Task.Delay(TimeSpan.FromMinutes(0.25));
                SomokatContext _context = new SomokatContext();

                var user = await _context.UserAccounts.FindAsync(requestBody.userId);
                var scooter = await _context.Scooters.FindAsync(requestBody.targetScooter);
                if (scooter.IdStatus != 4)
                {
                    return;
                }

                user.Bonus -= 1;
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

            scooter.IdStatus = 1; 

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("Pay")]
        public async Task<IActionResult> Pay([FromBody] UserPay requestBody)
        {
            SomokatContext _context = new SomokatContext();

            var user = await _context.UserAccounts.FindAsync(requestBody.userId);
            var value = requestBody.ValuePay;
            user.Bonus += value;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet]
        public IActionResult GetJsonData()
        {
            string jsonResult = null;
            using (var dbContext = new SomokatContext())
            {
                List<Scooter> scooters = dbContext.Scooters.Where(s => s.IdStatus == 1).ToList();

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

