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
                    List<UserAccount> userAccounts = dbContext.UserAccounts.ToList();

                    UserAccount authUser = userAccounts.SingleOrDefault(u => u.PhoneNumber == requestBody.PhoneNumber);

                    if (authUser == null)
                    {
                        return StatusCode(401, "Unauthorized: User not found");
                    }

                    // Ваша логика проверки пароля или других параметров, если необходимо

                    return StatusCode(200, "Authorized");
                }
        }


    }
    }


