using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyGymProject.Server.DTOs.Trainer;

namespace MyGymProject.Client.Pages
{
    public class ClientBookingModel : AuthorizedPageModel
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;
        [BindProperty]
        public string? Message { get; set; }

        [BindProperty]
        public List<TrainerReadDto>? Trainers { get; set; } 

        public ClientBookingModel(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor contextAccessor,
            IConfiguration configuration)
            : base(httpClientFactory, contextAccessor, configuration)
        {
            _configuration = configuration;
            _apiBaseUrl = _configuration["ApiBaseUrl"];
        }
        public async Task<IActionResult> OnGet()
        {
            var client = await LoadClientAsync();
            if (client == null)
                return RedirectToPage("/LoginPage");

            Trainers = await this._httpClient.GetFromJsonAsync<List<TrainerReadDto>>($"{_apiBaseUrl}/Trainers");

            if (!Trainers.Any())
            {
                Message = "Список тренеров пустой";
            }

            return Page();
            
        }
    }
}
