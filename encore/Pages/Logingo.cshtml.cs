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

        public string Title { get; set; } = "������";
        public string InfoMessage {  get; set; }
        public IActionResult OnGet(string source)
        {
            strUserName = GetUserSession("user_name");
            strUserNo = GetUserSession("user_no");

            if (string.IsNullOrEmpty(strUserName))
            {
                return RedirectToPage("/Loginmae");
            }
            WelcomeMessage = "�悤�����A" + strUserName + "����I";

            // source �ɉ����ĕ�����؂�ւ�
            InfoMessage = source switch
            {
                "register" => "�o�^���������܂����B���������������@�\���g���܂��B",
                "login" => "���O�C�����������܂����B�悤�����A����@�\�ցB",
                _ => "�悤�����B����@�\�������p���������܂��B"
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



