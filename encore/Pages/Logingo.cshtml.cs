using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace encore.Pages
{
    public class LogingoModel : PageModel
    {
        public string WelcomeMessage { get; set; }

        public IActionResult OnGet()
        {
            WelcomeMessage = "‚æ‚¤‚±‚»A‰ïˆõ‰æ–Ê‚ÖI";
            return Page();
        }
    }
}
