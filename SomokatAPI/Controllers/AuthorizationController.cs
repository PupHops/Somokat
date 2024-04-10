using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

using Somokat;

namespace SomokatAPI.Controllers
{
    public class AuthRequestBody
    {
        public string PhoneNumber { get; set; }

    }

    [ApiController]
    [Route("[controller]")]


    public class AuthorizationController : Controller
    {

        [HttpPost("login")]
        public IActionResult Login([FromBody] AuthRequestBody requestBody)
        {
            if (string.IsNullOrWhiteSpace(requestBody?.PhoneNumber))
            {
                return BadRequest("Phone number cannot be empty.");
            }

            using (var dbContext = new SomokatContext())
            {
                // Поиск пользователя по номеру телефона
                UserAccount authUser = dbContext.UserAccounts.FirstOrDefault(u => u.PhoneNumber == requestBody.PhoneNumber);

                if (authUser == null)
                {
                    return StatusCode(401, "Unauthorized: User not found");
                }

                // Ваша логика проверки пароля или других параметров, если необходимо

                // Возвращаем код 200 и количество бонусов пользователя
                return StatusCode(200, new { authUser.Bonus });
            }
        }



    }
}


