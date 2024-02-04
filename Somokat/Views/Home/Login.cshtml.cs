using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Newtonsoft.Json;
using System.Text;

namespace Somokat.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string PhoneNumber { get; set; }

        public void OnGet()
        {
        }
        public async Task<IActionResult> Login()
        {

            using (var httpClient = new HttpClient())
            {
                var apiEndpoint = "https://localhost:7209/Authorization/login"; // Замените на фактический адрес вашего API
                var requestData = new { PhoneNumber };
                var jsonRequestData = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(jsonRequestData, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(apiEndpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    var isAuthenticated = JsonConvert.DeserializeObject<bool>(apiResponse);

                    if (isAuthenticated)
                    {
                        // Пользователь успешно аутентифицирован
                        // Выполните необходимые действия
                        return RedirectToPage("/Index");
                    }
                    else
                    {
                        // Неверные учетные данные
                        ModelState.AddModelError(string.Empty, "Неверное имя пользователя или пароль");
                        return Page();
                    }
                }
                else
                {
                    // Обработка ошибок от API
                    ModelState.AddModelError(string.Empty, "Произошла ошибка при обращении к API");
                    return Page();
                }
            }
        }
    }
}
