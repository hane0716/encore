using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace encore.Pages
{
    public class LogingoModel : PageModel
    {
        public string WelcomeMessage { get; set; }

        public IActionResult OnGet()
        {
            WelcomeMessage = "�悤�����A�����ʂցI";
            return Page();
        }
    }
}
