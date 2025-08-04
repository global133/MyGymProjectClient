using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyGymProject.Server.DTOs.Client;
using MyGymProject.Server.DTOs.TrainingSession;
using System.Threading.Tasks;

namespace MyGymProject.Client.Pages
{
    public class AllSessionBookingModel : AuthorizedPageModel
    {
        [BindProperty]

        public ClientReadDto Client { get; set; }

        [BindProperty]

        public List<TrainingSessionReadDto> TrainingSessions { get; set; } = new List<TrainingSessionReadDto>();
        public AllSessionBookingModel(
            IHttpClientFactory httpClientFactory, 
            IHttpContextAccessor contextAccessor, 
            IConfiguration configuration) : 
            base(httpClientFactory, contextAccessor, configuration) 
        { }
        public async Task<ActionResult> OnGetAsync()
        {
            var clientData = await LoadClientAsync();
            if (clientData == null)
                return RedirectToPage("/LoginPage");


            TrainingSessions = await this._httpClient.GetFromJsonAsync<List<TrainingSessionReadDto>>($"{_apiBaseUrl}/trainings/all");

            return Page();
        }
    }
}
