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


        public void OnGet() { }

        /// <summary>
        /// なまえを登録ボタンを押下時
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public IActionResult OnPostInsert(string Name)
        {
            try
            {
                InsertUsers(Name);

                Message = Name + " でなまえを登録しました！";

                var user_no = NameToNo(Name);
                SetUserSession("user_name", Name);
                SetUserSession("user_no", user_no);

                return RedirectToPage("Logingo");
            }
            catch (Exception ex)
            {
                Message = "エラー: " + ex.Message;
                return Page(); // エラー時は元のページに残る
            }
        }

        /// <summary>
        /// ログインボタンを押下時
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public IActionResult OnPostLogin(string Name)
        {
            try
            {
                using var conn = new NpgsqlConnection(connString);
                conn.Open();
            }
            catch (Exception ex)
            {
                Message = "エラー: " + ex.Message;
                return Page(); // エラー時は元のページに残る
            }
            SetUserSession("user_name", Name);
            // ✅ 登録成功後に Logingo ページへリダイレクト
            return RedirectToPage("Logingo");
        }


        //public void GetUserId()
        //{
        //    try
        //    {
        //        using var conn = new NpgsqlConnection(connString);
        //        conn.Open();

        //        var sql = new StringBuilder();
        //        sql.AppendLine(" select user_id             ");
        //        sql.AppendLine("   from mst_users           ");
        //        sql.AppendLine("  where user_name = @name   ");
        //        sql.AppendLine("    and delete_date is null ");

        //        using var cmd = new NpgsqlCommand(sql.ToString(), conn);
        //        cmd.Parameters.AddWithValue("@name", Name);
        //        cmd.ExecuteNonQuery();

        //        var result = cmd.ExecuteScalar();

        //        if (result != null)
        //        {
        //            var UserId = result.ToString();
        //            SetUserSession("User_Id", UserId);
        //        }
        //        else
        //        {
        //            Message = "該当するユーザーが見つかりませんでした。";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Message = "エラー: " + ex.Message;
        //    }

        //}

        public void OnPostDelete()
        {
            try
            {
                using var conn = new NpgsqlConnection(connString);
                conn.Open();

                var sql = new StringBuilder();
                sql.AppendLine(" update mst_users                                             ");
                sql.AppendLine("    set edit_date   = date_trunc('second', current_timestamp) ");
                sql.AppendLine("      , delete_date = date_trunc('second', current_timestamp) ");
                sql.AppendLine("  where name = @name                                          ");

                using var cmd = new NpgsqlCommand(sql.ToString(), conn);
                cmd.Parameters.AddWithValue("@name", DelName);
                cmd.ExecuteNonQuery();

                Message = $"{DelName}　のなまえを削除しました。";
            }
            catch (Exception ex)
            {
                Message = "エラー: " + ex.Message;
            }
        }




    }
}
