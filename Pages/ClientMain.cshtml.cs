using Microsoft.AspNetCore.Mvc;
using MyGymProject.Server.DTOs.Client;
using System.Net.Http.Headers;

namespace MyGymProject.Client.Pages
{
    public class ClientMainModel : AuthorizedPageModel
    {

        [BindProperty]
        public ClientReadDto client { get; set; }

        [BindProperty]

        public ClientUpdateDto updatedClient { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool IsEditing { get; set; }

        [TempData]
        public string? Message { get; set; }

        public ClientMainModel(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor contextAccessor)
            :base(httpClientFactory, contextAccessor)
        { }


        public async Task<IActionResult> OnGetAsync()
        {
            var clientData = await LoadClientAsync();
            if (clientData == null)
                return RedirectToPage("/LoginPage");

            client = clientData;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var clientData = await LoadClientAsync();
            client = clientData;

            var response = await _httpClient.PutAsJsonAsync($"http://localhost:5155/api/Clients/{client.Id}", updatedClient);

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
