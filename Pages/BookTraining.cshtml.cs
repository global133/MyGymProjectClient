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
        [TempData]
        public string Messege {  get; set; }


        [BindProperty(SupportsGet = true)]
        public int TrainerId { get; set; }


        [BindProperty(SupportsGet = true)]
        public TrainerReadDto Trainer { get; set; }


        [BindProperty(SupportsGet = true)]
        public int WeekOffset { get; set; } = 0;

        private readonly string _apiBaseUrl;
        public List<DateTime> DaysOfWeek { get; set; } = new();
        public List<TrainingSessionReadDto> UpcomingSessions { get; set; } = new();
        public DateTime StartOfWeek { get; private set; }
        public DateTime EndOfWeek { get; private set; }

        public BookTrainingModel(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor contextAccessor,
            IConfiguration configuration) : base(httpClientFactory, contextAccessor, configuration)
        {
            _apiBaseUrl = configuration["ApiBaseUrl"];
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Trainer = await _httpClient.GetFromJsonAsync<TrainerReadDto>($"{_apiBaseUrl}/Trainers/{TrainerId}");

            var currentDate = DateTime.Today;
            StartOfWeek = currentDate.AddDays(-(int)currentDate.DayOfWeek + (int)DayOfWeek.Monday)
                          .AddDays(WeekOffset * 7);
            EndOfWeek = StartOfWeek.AddDays(7);

            DaysOfWeek = Enumerable.Range(0, 7)
                .Select(i => StartOfWeek.AddDays(i))
                .ToList();

            UpcomingSessions = await _httpClient.GetFromJsonAsync<List<TrainingSessionReadDto>>($"{_apiBaseUrl}/trainings/bytrainer/{Trainer.Id}");

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
            var response = await _httpClient.PostAsync($"{_apiBaseUrl}/trainings/{sessionId}/clients/{client.Id}", null);

            if (!response.IsSuccessStatusCode)
            {
                Messege = "Не удалось записаться на тренировку. Возможно вы уже записаны на эту тренировку";
                return RedirectToPage("/BookTraining", new {TrainerId = TrainerId});
            }

            TempData["Messege"] = "Вы успешно записались на тренировку!";
            return RedirectToPage("/ClientSchedule");
        }
    }
}