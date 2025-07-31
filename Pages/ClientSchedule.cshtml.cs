using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyGymProject.Server.DTOs.Client;
using MyGymProject.Server.DTOs.TrainingSession;
using MyGymProject.Server.Models;

namespace MyGymProject.Client.Pages
{
    public class ClientScheduleModel : AuthorizedPageModel
    {
        [TempData]
        public string Messege {  get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? SelectedDate { get; set; }

        [BindProperty]
        public List<TrainingSessionReadDto> Trainings { get; set; }
        public ClientScheduleModel(IHttpClientFactory httpClientFactory, IHttpContextAccessor contextAccessor, IConfiguration configuration) : 
            base(httpClientFactory, contextAccessor, configuration) 
        {}
        public async Task<IActionResult> OnGetAsync()
        {
            var clientData = await LoadClientAsync();
            if (clientData == null)
                return RedirectToPage("/LoginPage");

            var token = Request.Cookies["jwt"];

            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/LoginPage");


            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Clients/schedule{clientData.Id}");
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Ошибка: {response.StatusCode}");
            }

            var allTrainings = await response.Content.ReadFromJsonAsync<List<TrainingSessionReadDto>>();

            Trainings = SelectedDate.HasValue
                ? allTrainings
                    .Where(t => t.StartTime.Date == SelectedDate.Value.Date)
                    .OrderBy(t => t.StartTime)
                    .ToList()
                : allTrainings
                    .OrderBy(t => t.StartTime)
                    .ToList();

            return Page();
        }
        public async Task<IActionResult> OnPostDeleteAsync(int sessionId)
        {
            var client = await LoadClientAsync(); 
            var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/trainings/{sessionId}/clients/{client.Id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Не удалось отменить запись на тренировку.";
            }
            else
            {
                TempData["Success"] = "Запись успешно отменена.";
            }

            return RedirectToPage(); 
        }
    }
}
