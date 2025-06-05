using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyGymProject.Server.DTOs.Client;
using MyGymProject.Server.DTOs.Training;
using MyGymProject.Server.Models;

namespace MyGymProject.Client.Pages
{
    public class ClientScheduleModel : AuthorizedPageModel
    {
        [BindProperty(SupportsGet = true)]
        public DateTime? SelectedDate { get; set; }

        [BindProperty]
        public List<TrainingResponseDTO> Trainings { get; set; }
        public ClientScheduleModel(HttpClient httpClient) : base(httpClient) { }
        public async Task<IActionResult> OnGetAsync()
        {
            var clientData = await LoadClientAsync();
            if (clientData == null)
                return RedirectToPage("/LoginPage");

            var token = Request.Cookies["jwt"];

            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/LoginPage");


            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5155/api/Trainings/my-schedule");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Ошибка: {response.StatusCode}");
            }

            var allTrainings = await response.Content.ReadFromJsonAsync<List<TrainingResponseDTO>>();

            if (SelectedDate.HasValue)
            {
                Trainings = allTrainings
                    .Where(t => t.Time.Date == SelectedDate.Value.Date)
                    .OrderBy(t => t.Time)
                    .ToList();
            }
            else
            {
                Trainings = allTrainings.OrderBy(t => t.Time).ToList();
            }

            return Page();
        }
    }
}
