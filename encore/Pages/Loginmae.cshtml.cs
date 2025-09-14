using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Data;


namespace encore.Pages
{
    public class LoginmaeModel : BasePageModel
    {
        public string Title { get; set; } = "会員登録画面";

        private readonly string connString = "Host=localhost;Username=postgres;Password=encore;Database=postgres";

        [BindProperty]
        public string Name { get; set; }

        [BindProperty]
        public string DelName { get; set; }

        public DataTable NameList { get; set; }

        public string UserId {  get; set; }

        [BindProperty]
        public string RegisterName { get; set; }

        [BindProperty]
        public string LoginName { get; set; }


        /// <summary>
        /// Page_Loadの役割
        /// </summary>
        public void OnGet() {
            ClearUserSession("user_name");
            ClearUserSession("user_no");
        }

        /// <summary>
        /// なまえを登録ボタンを押下時
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public IActionResult OnPostInsert()
        {
            try
            {
                InsertUsers(RegisterName);

                Message = Name + " でなまえを登録しました！";

                var user_no = NameToNo(RegisterName);
                SetUserSession("user_name", RegisterName);
                SetUserSession("user_no", user_no);

                return RedirectToPage("Logingo", new {source = "register"});
            }
            catch (Exception ex)
            {
                Message = "エラー: " + ex.Message;
                RegisterName = ""; // 入力欄を空にする
                return Page();
            }
        }

        /// <summary>
        /// ログインボタンを押下時
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public IActionResult OnPostLogin()
        {
            try
            {
                string UserNo = NameToNo(LoginName);
                if (CheckUser(UserNo)) {
                    SetUserSession("user_no", UserNo);
                    SetUserSession("user_name", LoginName);
                    return RedirectToPage("Logingo", new { source = "login"});
                }
                else
                {
                    Message = "なまえ " + LoginName + " が間違っています";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                Message = "エラー: " + ex.Message;
                LoginName = ""; // 入力欄を空にする
                return Page();
            }
        }


        ///// <summary>
        ///// ユーザーネームを削除するボタンを押下時
        ///// </summary>
        //public void OnPostDelete()
        //{
        //    try
        //    {
        //        using var conn = new NpgsqlConnection(connString);
        //        conn.Open();

        //        var sql = new StringBuilder();
        //        sql.AppendLine(" update mst_users                                             ");
        //        sql.AppendLine("    set edit_date   = date_trunc('second', current_timestamp) ");
        //        sql.AppendLine("      , delete_date = date_trunc('second', current_timestamp) ");
        //        sql.AppendLine("  where name = @name                                          ");

        //        using var cmd = new NpgsqlCommand(sql.ToString(), conn);
        //        cmd.Parameters.AddWithValue("@name", DelName);
        //        cmd.ExecuteNonQuery();

        //        Message = $"{DelName}　のなまえを削除しました。";
        //    }
        //    catch (Exception ex)
        //    {
        //        Message = "エラー: " + ex.Message;
        //    }
        //}




    }
}
