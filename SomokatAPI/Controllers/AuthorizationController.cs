using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

using Somokat;

namespace SomokatAPI.Controllers
{
    public class AuthRequestBody
    {
        public string PhoneNumber{ get; set; }
        
    }

    [ApiController]
    [Route("[controller]")]


    public class AuthorizationController : Controller
    {

        [HttpPost("login")]
        public IActionResult Login([FromBody] AuthRequestBody PhoneNumber)
        {
            using (var dbContext = new SomokatContext())
            {
                // Получение списка скутеров из базы данных
                List<UserAccount> _UserAccount = dbContext.UserAccounts.ToList();
                var auth = _UserAccount.SingleOrDefault(u => u.PhoneNumber == PhoneNumber.PhoneNumber);
                if (auth == null)
                {
                    return StatusCode(401);
                }
                return StatusCode(200);
            }
            // Поиск пользователя в базе данных

        
        }
    }
}
