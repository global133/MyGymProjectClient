using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyGymProject.Client.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnPostAsync()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToPage("/LoginPage");
        }
    }
}
