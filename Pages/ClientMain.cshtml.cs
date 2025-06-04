using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using MyGymProject.Server.DTOs;
using Application.DTOs.Client;

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
