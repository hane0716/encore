using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;
using System.Data;
using System.Text;

namespace encore.Pages
{


    public class KirokuModel : BasePageModel
    {
        private readonly string connString = "Host=localhost;Username=postgres;Password=encore;Database=postgres";

        public string Title { get; set; } = "ライブ記録画面";



        public void OnGet()
        {
            strUserName = GetUserSession("user_name");
            KaiinMessage = strUserName + "さんのページです";
            LiveDate = DateTime.Today; // ← これで初期表示が今日になる
        }


        public IActionResult OnPostInsert_Kiroku(string KirokuName,DateTime LiveDate)
        {
            try
            {
                strUserName = GetUserSession("user_name");
                using var conn = new NpgsqlConnection(connString);
                conn.Open();

                var sbsql = new StringBuilder();
                sbsql.AppendLine("insert into mst_kiroku(");
                sbsql.AppendLine("     user_id, live_name, live_date, create_date, edit_date");
                sbsql.AppendLine(") values (");
                sbsql.AppendLine("    @user_name, @live_name, @live_date, date_trunc('second', current_timestamp), date_trunc('second', current_timestamp)");
                sbsql.AppendLine(")");

                using var cmd = new NpgsqlCommand(sbsql.ToString(), conn);
                cmd.Parameters.AddWithValue("@user_name", strUserName);
                cmd.Parameters.AddWithValue("@live_name", KirokuName);
                cmd.Parameters.AddWithValue("@live_date", LiveDate);
                cmd.ExecuteNonQuery();

                Message = $"{LiveDate:yyyy-MM-dd} 開催の {KirokuName} を登録しました！";
            }
            catch (Exception ex)
            {
                Message = "エラー: " + ex.Message;
            }

            return Page();

        }
    }

    
}
