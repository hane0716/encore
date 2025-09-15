using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;
using System.Data;
using System.Diagnostics.Tracing;
using System.Text;


namespace encore.Pages
{
    public class LogingoModel : BasePageModel
    {
        private readonly string connString = "Host=localhost;Username=postgres;Password=encore;Database=postgres";

        public string Title { get; set; } = "会員画面";
        public string InfoMessage {  get; set; }
        public IActionResult OnGet(string source)
        {
            strUserName = GetUserSession("user_name");
            strUserNo = GetUserSession("user_no");

            if (string.IsNullOrEmpty(strUserName))
            {
                return RedirectToPage("/Loginmae");
            }
            WelcomeMessage = "ようこそ、" + strUserName + "さん！";

            // source に応じて文言を切り替え
            InfoMessage = source switch
            {
                "register" => "登録が完了しました。ここから会員向け機能を使えます。",
                "login" => "ログインが完了しました。ようこそ、会員機能へ。",
                _ => "ようこそ。会員機能をご利用いただけます。"
            };
            return Page();
        }

        public IActionResult OnPostLogout()
        {
            ClearUserSession("user_no");
            ClearUserSession("user_name");
            return RedirectToPage("Loginmae");
        }

    }
}



