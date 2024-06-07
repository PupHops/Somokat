using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SomokatAPI;

namespace SomokatAPI.Controllers
{
    public class AuthRequestBody
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }


    }
    public class SaveNameRequestBody
    {
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }


    }
    public class RegRequestBody
    {
        public string PhoneNumber { get; set; }
        public string Password { get; set; }


    }
    public class CheckRequestBody
    {
        public int userId{ get; set; }

    }
    [ApiController]
    [Route("[controller]")]


    public class AuthorizationController : Controller
    {
        [HttpPost("CheckMoney")]

        public IActionResult CheckMoney([FromBody] CheckRequestBody requestBody)
        {
            SomokatContext context= new SomokatContext();
            try
            {
                UserAccount authUser = context.UserAccounts.FirstOrDefault(u => u.Id == requestBody.userId);
                return StatusCode(200, new { authUser.balance, authUser.Bonus });

            }
            catch
            {
                return NoContent();
            }



        }
        [HttpPost("SaveNameSurname")]
        public async Task<IActionResult> SaveName([FromBody] SaveNameRequestBody requestBody)
        {
            SomokatContext context = new SomokatContext();
            
            var user =  await context.UserAccounts.FirstOrDefaultAsync(u => u.PhoneNumber == requestBody.PhoneNumber);
            user.name = requestBody.Name;
            user.surname = requestBody.Surname;
            context.UserAccounts.Update(user);
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] AuthRequestBody requestBody)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(requestBody?.PhoneNumber))
                {
                    return BadRequest("Телефон не должен быть пустым");
                }

                using (var dbContext = new SomokatContext())
                {
                    UserAccount authUser = dbContext.UserAccounts.FirstOrDefault(u => u.PhoneNumber == requestBody.PhoneNumber && u.Password == requestBody.Password);

                    if (authUser == null)
                    {
                        return StatusCode(401, "Пользователь не найден");
                    }
                    return StatusCode(200, new { authUser.balance,authUser.Bonus, authUser.Id,authUser.name,authUser.surname });
                }
            }
            catch {
                return StatusCode(401, "Пользователь не найден");
            }
        }

        [HttpPost("Registration")]
        public IActionResult Registration([FromBody] RegRequestBody requestBody)
        {
            using (var dbContext = new SomokatContext())
            {
                UserAccount authUser = dbContext.UserAccounts.FirstOrDefault(u => u.PhoneNumber == requestBody.PhoneNumber);

                int maxid = dbContext.UserAccounts.Max(u=>u.Id);

                if (authUser != null)
                {
                    return StatusCode(401, "Пользователь уже существует");
                }
                UserAccount userAccount = new UserAccount();
                userAccount.Id = maxid+1;
                userAccount.PhoneNumber=requestBody.PhoneNumber;
                userAccount.Password = requestBody.Password;
                userAccount.Bonus = 1000;
                dbContext.UserAccounts.Add(userAccount);
                dbContext.SaveChanges();
                return StatusCode(200);
            }
        }

    }
}


