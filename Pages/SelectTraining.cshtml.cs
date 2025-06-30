using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyGymProject.Client.Pages
{
    public class SelectTrainingModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int trainerId { get; set; }
        public void OnGet()
        {
        }
    }
}
