using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;


namespace encore.Pages
{
    public class LogingoModel : BasePageModel
    {
        public string Title { get; set; } = "‰ïˆõ‰æ–Ê";
        public IActionResult OnGet()
        {
            var user_name = GetUserSession("user_name");

            WelcomeMessage = "‚æ‚¤‚±‚»A" + user_name + "‚³‚ñI";
            return Page();
        }
    }
}
