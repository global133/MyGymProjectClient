
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyGymProject.Server.DTOs.Client;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace MyGymProject.Client
{
    public class AuthorizedPageModel : PageModel
    {
        private readonly IConfiguration _configuration;

        protected readonly string _apiBaseUrl;
        protected readonly HttpClient _httpClient;

        public AuthorizedPageModel(IHttpClientFactory httpClientFactory, IHttpContextAccessor contextAccessor, IConfiguration configuration)
        {
            _configuration = configuration;
            _apiBaseUrl = _configuration["ApiBaseUrl"];

            _httpClient = httpClientFactory.CreateClient();

            var token = contextAccessor.HttpContext?.Request.Cookies["jwt"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }


        protected async Task<ClientReadDto?> LoadClientAsync()
        {
            var token = Request.Cookies["jwt"];
            if (string.IsNullOrEmpty(token)) return null;

            var login = GetLoginFromToken(token);
            if (string.IsNullOrEmpty(login)) return null;

            var id = GetClientIdFromToken(token);

            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Clients/{id}");
            if (!response.IsSuccessStatusCode) return null;

            return await response.Content.ReadFromJsonAsync<ClientReadDto>();
        }

        private string GetLoginFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            return jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? string.Empty;
        }
        private int? GetClientIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var idClaim = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(idClaim, out int clientId))
            {
                return clientId;
            }

            return null; 
        }

    }
}
