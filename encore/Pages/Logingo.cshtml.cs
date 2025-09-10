using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;
using System.Data;
using System.Text;


namespace encore.Pages
{
    public class LogingoModel : BasePageModel
    {
        private readonly string connString = "Host=localhost;Username=postgres;Password=encore;Database=postgres";

        public string Title { get; set; } = "âÔàıâÊñ ";
        public IActionResult OnGet()
        {
            strUserName = GetUserSession("user_name");
            strUserNo = GetUserSession("user_no");

            WelcomeMessage = "ÇÊÇ§Ç±ÇªÅA" + strUserName + "Ç≥ÇÒÅI";
            return Page();
        }

    }
}
