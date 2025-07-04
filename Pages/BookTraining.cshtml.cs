using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using MyGymProject.Server.DTOs.Trainer;
using MyGymProject.Server.DTOs.Training;
using MyGymProject.Server.Models;

namespace MyGymProject.Client.Pages
{
    public class BookTrainingModel : AuthorizedPageModel
    {
        [BindProperty(SupportsGet = true)]
        public int TrainerId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int WeekOffset { get; set; } = 0;

        [BindProperty(SupportsGet = true)]
        public string TrainingName { get; set; }

        public TrainerReadDto Trainer { get; set; }
        public List<string> TimeSlots { get; set; } = new()
        {
            "08:00", "10:00", "12:00", "14:00", "16:00", "18:00"
        };

        public Dictionary<DayOfWeek, Dictionary<string, TrainingResponseDTO>> WeeklySchedule { get; set; } = new();
        public DateTime StartOfWeek { get; private set; }
        public DateTime EndOfWeek { get; private set; }

        private readonly IMemoryCache _cache;
        public BookTrainingModel(
            IHttpClientFactory httpClientFactory, 
            IHttpContextAccessor contextAccessor, 
            IMemoryCache memoryCache) : base(httpClientFactory, contextAccessor)
        { _cache = memoryCache; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!_cache.TryGetValue($"trainer_{TrainerId}", out TrainerReadDto trainer))
            {
                trainer = await _httpClient.GetFromJsonAsync<TrainerReadDto>($"http://localhost:5155/api/Trainers/{TrainerId}");
                _cache.Set($"trainer_{TrainerId}", trainer, TimeSpan.FromMinutes(10));
            }

            var schedule = await this._httpClient.GetFromJsonAsync<List<TrainingResponseDTO>>($"http://localhost:5155/api/trainings/{TrainingName}trainer/{TrainerId}");

            var currentDate = DateTime.Today;
            StartOfWeek = currentDate.AddDays(-(int)currentDate.DayOfWeek + (int)DayOfWeek.Monday)
                          .AddDays(WeekOffset * 7);
            EndOfWeek = StartOfWeek.AddDays(6);

            var daysOfWeek = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>();
            foreach (var day in daysOfWeek)
            {
                WeeklySchedule[day] = new Dictionary<string, TrainingResponseDTO>();
                foreach (var timeSlot in TimeSlots)
                {
                    WeeklySchedule[day][timeSlot] = null; 
                }
            }

            foreach (var training in schedule.Where(t => t.Time.Date >= StartOfWeek && t.Time.Date <= EndOfWeek))
            {
                var dayOfWeek = training.Time.DayOfWeek;
                var time = training.Time.ToString("HH:mm");

                if (WeeklySchedule.ContainsKey(dayOfWeek) && WeeklySchedule[dayOfWeek].ContainsKey(time))
                {
                    WeeklySchedule[dayOfWeek][time] = training;
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = await LoadClientAsync();
            var response = await this._httpClient.PostAsJsonAsync($"http://localhost:5155/api/trainings/{TrainerId}/clients/{client.Id}", new { });

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "�� ������� ����������.";
                return RedirectToPage(new { TrainerId });
            }

            TempData["Success"] = "�� ������� ����������!";
            return RedirectToPage("/ClientSchedule"); 
        }
    }
}
