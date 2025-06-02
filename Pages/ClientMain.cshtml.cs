using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using MyGymProject.Server.DTOs;
using Application.DTOs.Client;

namespace MyGymProject.Client.Pages
{
    public class ClientMainModel : PageModel
    {
        private readonly HttpClient _httpClient;

        [BindProperty]
        public ClientReadDto client { get; set; }

        public ClientMainModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = Request.Cookies["jwt"];
            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/LoginPage");

            this._httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var login = GetLoginFromToken(token);
            if (string.IsNullOrEmpty(login))
                return RedirectToPage("/LoginPage");

            var response = await this._httpClient.GetAsync($"http://localhost:5155/api/Clients/bylogin/{login}");
            if (!response.IsSuccessStatusCode)
                return RedirectToPage("/LoginPage");

            client = await response.Content.ReadFromJsonAsync<ClientReadDto>();
            return Page();
        }

        private string GetLoginFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            return jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? string.Empty;
        }
    }
}
