using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyGymProject.Server.DTOs.Trainer;
using MyGymProject.Server.DTOs.Training;
using MyGymProject.Server.Models;

namespace MyGymProject.Client.Pages
{
    public class BookTrainingModel : AuthorizedPageModel
    {
        [BindProperty(SupportsGet = true)]
        public int TrainerId { get; set; }

        [BindProperty]
        public int SelectedTrainingId { get; set; }

        public TrainerReadDto Trainer { get; set; }

        public List<string> TimeSlots { get; set; } = new()
        {
            "08:00", "10:00", "12:00", "14:00", "16:00", "18:00"
        };

        public Dictionary<DateTime, Dictionary<string, TrainingResponseDTO>> SlotMatrix { get; set; } = new();
        public BookTrainingModel(
            IHttpClientFactory httpClientFactory, 
            IHttpContextAccessor contextAccessor) : base(httpClientFactory, contextAccessor)
        {}

        public async Task<IActionResult> OnGetAsync()
        {
            Trainer = await this._httpClient.GetFromJsonAsync<TrainerReadDto>($"http://localhost:5155/api/Trainers/{TrainerId}");

            var schedule = await this._httpClient.GetFromJsonAsync<List<TrainingResponseDTO>>($"http://localhost:5155/api/trainings/trainer/{TrainerId}");

            var days = schedule
                .Select(t => t.Time.Date)
                .Distinct()
                .OrderBy(d => d)
                .Take(7); // показываем 7 дней

            foreach (var day in days)
            {
                SlotMatrix[day] = new Dictionary<string, TrainingResponseDTO>();
                foreach (var slot in schedule.Where(t => t.Time.Date == day))
                {
                    var time = slot.Time.ToString("HH:mm");
                    SlotMatrix[day][time] = slot;
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var clientId = await LoadClientAsync();
            var response = await this._httpClient.PostAsJsonAsync($"http://localhost:5155/api/trainings/{TrainerId}/clients/{clientId}", new { });

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Ќе удалось записатьс€.";
                return RedirectToPage(new { TrainerId });
            }

            TempData["Success"] = "¬ы успешно записались!";
            return RedirectToPage("/ClientSchedule"); // или другую страницу
        }
    }
}
