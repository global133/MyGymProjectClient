using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyGymProject.Server.DTOs.Trainer;

namespace MyGymProject.Client.Pages
{
    public class ClientBookingModel : AuthorizedPageModel
    {

        [BindProperty]
        public string? Message { get; set; }

        [BindProperty]
        public List<TrainerReadDto>? Trainers { get; set; } 

        public ClientBookingModel(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor contextAccessor)
            : base(httpClientFactory, contextAccessor)
        { }
        public async Task<IActionResult> OnGet()
        {
            var client = await LoadClientAsync();
            if (client == null)
                return RedirectToPage("/LoginPage");

            Trainers = await this._httpClient.GetFromJsonAsync<List<TrainerReadDto>>("http://localhost:5155/api/Trainers");

            if (!Trainers.Any())
            {
                Message = "Список тренеров пустой";
            }

            return Page();
            
        }
    }
}
