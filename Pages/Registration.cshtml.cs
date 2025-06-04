using Application.DTOs.Client;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace MyGymProject.Client.Pages
{
    public class RegistrationModel : PageModel
    {
        private readonly HttpClient _httpClient;

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

        public RegistrationModel(HttpClient httpClient)
        {
            this._httpClient = httpClient;
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
                var response = await _httpClient.PostAsJsonAsync("http://localhost:5155/api/Auth/register", clientCreateDto);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("/LoginPage");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    ErrorMessage = "Ошибка при регистрации: такой пользователь уже существует";
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
