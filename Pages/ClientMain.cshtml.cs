using Microsoft.AspNetCore.Mvc;
using MyGymProject.Server.DTOs.Client;
using MyGymProject.Server.DTOs.TrainingSession;
using System.Net.Http.Headers;

namespace MyGymProject.Client.Pages
{
    public class ClientMainModel : AuthorizedPageModel
    {

        [BindProperty]
        public ClientReadDto client { get; set; }

        [BindProperty]
        public List<TrainingSessionReadDto> trainingSessions { get; set; }

        [BindProperty]

        public ClientUpdateDto updatedClient { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool IsEditing { get; set; }

        [TempData]
        public string? Message { get; set; }
   
        public ClientMainModel(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor contextAccessor,
            IConfiguration configuration)
            :base(httpClientFactory, contextAccessor, configuration)
        { }


        public async Task<IActionResult> OnGetAsync()
        {
            var clientData = await LoadClientAsync();
            if (clientData == null)
                return RedirectToPage("/LoginPage");

            client = clientData;

            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Clients/schedule{client.Id}");

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Ошибка: {response.StatusCode}");
            }

            trainingSessions = await response.Content.ReadFromJsonAsync<List<TrainingSessionReadDto>>();
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var clientData = await LoadClientAsync();
            client = clientData;

            var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/Clients/{client.Id}", updatedClient);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Ошибка при сохранении данных.");
                return Page();
            }

            Message = "Данные сохранены";
            return RedirectToPage("/ClientMain");
        }
    }
}
