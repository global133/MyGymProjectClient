using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using MyGymProject.Server.DTOs.Trainer;
using MyGymProject.Server.DTOs.TrainingSession;
using MyGymProject.Server.Models;


namespace MyGymProject.Client.Pages
{
    public class BookTrainingModel : AuthorizedPageModel
    {

        [BindProperty(SupportsGet = true)]
        public int TrainerId { get; set; }

        [BindProperty(SupportsGet = true)]
        public TrainerReadDto Trainer { get; set; }


        [BindProperty(SupportsGet = true)]
        public int WeekOffset { get; set; } = 0;

        public List<DateTime> DaysOfWeek { get; set; } = new();
        public List<TrainingSessionReadDto> UpcomingSessions { get; set; } = new();
        public DateTime StartOfWeek { get; private set; }
        public DateTime EndOfWeek { get; private set; }

        private readonly IMemoryCache _cache;

        public BookTrainingModel(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor contextAccessor,
            IMemoryCache memoryCache) : base(httpClientFactory, contextAccessor)
        {
            _cache = memoryCache;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!_cache.TryGetValue($"trainer_{TrainerId}", out TrainerReadDto trainer))
            {
                trainer = await _httpClient.GetFromJsonAsync<TrainerReadDto>($"http://localhost:5155/api/Trainers/{TrainerId}");
                _cache.Set($"trainer_{TrainerId}", trainer, TimeSpan.FromMinutes(10));
            }
            Trainer = trainer;

            var currentDate = DateTime.Today;
            StartOfWeek = currentDate.AddDays(-(int)currentDate.DayOfWeek + (int)DayOfWeek.Monday)
                          .AddDays(WeekOffset * 7);
            EndOfWeek = StartOfWeek.AddDays(7);

            DaysOfWeek = Enumerable.Range(0, 7)
                .Select(i => StartOfWeek.AddDays(i))
                .ToList();

            UpcomingSessions = await _httpClient.GetFromJsonAsync<List<TrainingSessionReadDto>>($"http://localhost:5155/api/trainings/bytrainer/{Trainer.Id}");

            // Фильтрация по текущей неделе
            UpcomingSessions = UpcomingSessions?
                .Where(s => s.StartTime >= StartOfWeek && s.StartTime <= EndOfWeek)
                .OrderBy(s => s.StartTime)
                .ToList() ?? new List<TrainingSessionReadDto>();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int sessionId)
        {
            var client = await LoadClientAsync();
            var response = await _httpClient.PostAsJsonAsync(
                $"api/trainings/{sessionId}/clients/{client.Id}",
                new { });

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Не удалось записаться на тренировку.";
                return RedirectToPage(new { TrainerId, WeekOffset });
            }

            TempData["Success"] = "Вы успешно записались на тренировку!";
            return RedirectToPage("/ClientSchedule");
        }
    }
}