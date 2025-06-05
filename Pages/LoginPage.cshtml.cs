
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace MyGymProject.Client.Pages
{
    public class LoginPageModel : AuthorizedPageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "Введите логин")]
        public string Login { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Введите пароль")]
        public string Password { get; set; }
        public string ErrorMessage { get; set; }

        public LoginPageModel(HttpClient httpClient) : base(httpClient) { }
      
        public async Task<IActionResult> OnGetAsync()
        {
            var client = await LoadClientAsync();
            if (client == null)
                return Page();

            return RedirectToPage("/ClientMain");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var data = new { Login = this.Login, Password = this.Password };
            try
            {
                var response = await this._httpClient.PostAsJsonAsync("http://localhost:5155/api/Auth/login", data);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadFromJsonAsync<JsonElement>();
                    string token = json.GetProperty("token").GetString();
                    string role = json.GetProperty("role").GetString();

                    Response.Cookies.Append("jwt", token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.UtcNow.AddHours(5)
                    });

                    return Redirect("/ClientMain");
                }

                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    ErrorMessage = "Ошибка авторизации неверный логин или пароль";
                    return Page();
                }

                else
                {
                    ErrorMessage = "Неверный логин или пароль";
                    return Page();
                }
            }
            catch (HttpRequestException ex)
            {
                ErrorMessage = $"Ошибка: {ex.Message}";
                return Page();
            }


        }
    }
}
