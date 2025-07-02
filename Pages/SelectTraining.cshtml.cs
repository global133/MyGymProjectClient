using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using MyGymProject.Client.Services;
using MyGymProject.Server.DTOs.Trainer;
using MyGymProject.Server.DTOs.Training;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyGymProject.Client.Pages
{
    public class SelectTrainingModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int TrainerId { get; set; }

        [BindProperty]
        public List<TrainingResponseDTO>? Trainings { get; set; }

        private readonly HttpClient _httpClient;
        private readonly CacheService _cacheService;

        [BindProperty]
        public string ErrorMessege {  get; set; }
        public SelectTrainingModel(IHttpClientFactory httpClientFactory, CacheService cacheService) 
        { 
            this._httpClient = httpClientFactory.CreateClient();
            this._cacheService = cacheService;
        }
        public async Task<IActionResult> OnGet()
        {
            Trainings = await _cacheService.GetOrSetAsync(
            $"trainings_{TrainerId}",
            () => _httpClient.GetFromJsonAsync<List<TrainingResponseDTO>>($"http://localhost:5155/api/Trainings/trainer/{TrainerId}"),
            TimeSpan.FromMinutes(10)
        );

            if ( Trainings == null )
            {
                ErrorMessege = "Не удалось получить данные";
            }
            return Page();
         
        }
    }
}
