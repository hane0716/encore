using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;


namespace encore.Pages
{
    public class LogingoModel : BasePageModel
    {
        public string WelcomeMessage { get; set; }

        public IActionResult OnGet()
        {
            var user_name = GetUserSession("user_name");

            WelcomeMessage = "ÇÊÇ§Ç±ÇªÅA" + user_name + "Ç≥ÇÒÅI";
            return Page();
        }
    }
}
