using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyGymProject.Server.DTOs.Client;
using System.Threading.Tasks;

namespace MyGymProject.Client.Pages
{
    public class AllSessionBookingModel : AuthorizedPageModel
    {
        [BindProperty]

        public ClientReadDto Client { get; set; }

        public AllSessionBookingModel(
            IHttpClientFactory httpClientFactory, 
            IHttpContextAccessor contextAccessor, 
            IConfiguration configuration) : 
            base(httpClientFactory, contextAccessor, configuration) 
        { }
        public async Task<ActionResult> OnGetAsync()
        {
            var clientData = await LoadClientAsync();
            if (clientData == null)
                return RedirectToPage("/LoginPage");

            
            

            return Page();
        }
    }
}
