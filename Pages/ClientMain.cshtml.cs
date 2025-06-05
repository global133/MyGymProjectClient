using Microsoft.AspNetCore.Mvc;
using MyGymProject.Server.DTOs.Client;

namespace MyGymProject.Client.Pages
{
    public class ClientMainModel : AuthorizedPageModel
    {

        [BindProperty]
        public ClientReadDto client { get; set; }

        public ClientMainModel(HttpClient httpClient) : base(httpClient) { }
     

        public async Task<IActionResult> OnGetAsync()
        {
            var clientData = await LoadClientAsync();
            if (clientData == null)
                return RedirectToPage("/LoginPage");

            client = clientData;
            return Page();
        }
    }
}
