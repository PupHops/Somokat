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
                var apiEndpoint = "https://localhost:7209/Authorization/login"; // �������� �� ����������� ����� ������ API
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
                        // ������������ ������� ����������������
                        // ��������� ����������� ��������
                        return RedirectToPage("/Index");
                    }
                    else
                    {
                        // �������� ������� ������
                        ModelState.AddModelError(string.Empty, "�������� ��� ������������ ��� ������");
                        return Page();
                    }
                }
                else
                {
                    // ��������� ������ �� API
                    ModelState.AddModelError(string.Empty, "��������� ������ ��� ��������� � API");
                    return Page();
                }
            }
        }
    }
}
