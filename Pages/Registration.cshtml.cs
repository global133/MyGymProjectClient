
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyGymProject.Server.DTOs.Client;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace MyGymProject.Client.Pages
{
    public class RegistrationModel : PageModel
    {
        private readonly HttpClient _httpClient;

        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;

        [BindProperty]
        [Required(ErrorMessage = "Ведите логин")]
        [Display(Name = "Имя")]
        public string Name { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Выберите дату рождения")]
        public DateTime BirthDate { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Введите номер телефона")]
        public string Phone { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Введите email")]
        public string Email { get; set; }

        [BindProperty]
        public string Gender { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Введите логин")]
        public string Login { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Введите пароль")]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public RegistrationModel(HttpClient httpClient, IConfiguration configuration)
        {
            this._httpClient = httpClient;
            this._configuration = configuration;
            this._apiBaseUrl = _configuration["ApiBaseUrl"];
        }
        public void OnGet()
        {

        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var clientCreateDto = new ClientCreateDto
            {
                FullName = this.Name,
                DateOfBirth = this.BirthDate,
                Phone = this.Phone,
                Email = this.Email,
                Gender = this.Gender,
                Login = this.Login,
                Password = this.Password
            };

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/Auth/register", clientCreateDto);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("/LoginPage");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    var problemDetails = JsonSerializer.Deserialize<ValidationProblemDetails>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (problemDetails?.Errors != null && problemDetails.Errors.Any())
                    {
                        ErrorMessage = string.Join(" ", problemDetails.Errors.SelectMany(e => e.Value));
                    }
                    else
                    {
                        ErrorMessage = "Ошибка при регистрации: логин уже существует или введены некорректные данные";
                    }

                    return Page();
                }
                else
                {
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Ошибка сервера: " + ex.Message;
                return Page();
            }
        }
    }
}
