using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
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

        public List<(int No, string Name, DateTime Date)> KirokuList { get; set; } = new();

        public void OnGet()
        {
            strUserName = GetUserSession("user_name");
            strUserNo = GetUserSession("user_no");
            KaiinMessage = strUserName + "さんのページです";
            LiveDate = DateTime.Today; // ← これで初期表示が今日になる

            KirokuList = GetKirokuList(strUserNo);
        }


        public IActionResult OnPostInsert_Kiroku(string KirokuName,DateTime LiveDate)
        {
            try
            {
                strUserNo = GetUserSession("user_no");
                using var conn = new NpgsqlConnection(connString);
                conn.Open();

                var sbsql = new StringBuilder();
                sbsql.AppendLine(" insert into mst_kiroku(                        ");
                sbsql.AppendLine("        user_no                                 ");
                sbsql.AppendLine("      , live_name                               ");
                sbsql.AppendLine("      , live_date                               ");
                sbsql.AppendLine("      , create_date                             ");
                sbsql.AppendLine("      , edit_date                               ");
                sbsql.AppendLine(" )                                              ");
                sbsql.AppendLine(" values (                                       ");
                sbsql.AppendLine("        @user_no                                ");
                sbsql.AppendLine("      , @live_name                              ");
                sbsql.AppendLine("      , @live_date                              ");
                sbsql.AppendLine("      , date_trunc('second', current_timestamp) ");
                sbsql.AppendLine("      , date_trunc('second', current_timestamp) ");
                sbsql.AppendLine(" )                                              ");

                using var cmd = new NpgsqlCommand(sbsql.ToString(), conn);
                cmd.Parameters.AddWithValue("@user_no", strUserNo);
                cmd.Parameters.AddWithValue("@live_name", KirokuName);
                cmd.Parameters.AddWithValue("@live_date", LiveDate);
                cmd.ExecuteNonQuery();

                Message = $"{LiveDate:yyyy-MM-dd} 開催の {KirokuName} を登録しました！";
            }
            catch (Exception ex)
            {
                Message = "エラー: " + ex.Message;
            }

            KirokuList = GetKirokuList(strUserNo);
            return Page();

        }


        private List<(int No, string Name, DateTime Date)> GetKirokuList(string userNo)
        {
            var List = new List<(int, string, DateTime)>();
            try
            {
                using var conn = new NpgsqlConnection(connString);
                conn.Open();

                StringBuilder sbSql = new StringBuilder();
                sbSql.AppendLine(" select kiroku_no, live_name, live_date ");
                sbSql.AppendLine("   from mst_kiroku           ");
                sbSql.AppendLine("  where user_no = @user_no   ");
                sbSql.AppendLine("    and delete_date is null  ");
                sbSql.AppendLine("  order by live_date desc    ");

                using var cmd = new NpgsqlCommand(sbSql.ToString(), conn);
                cmd.Parameters.AddWithValue("@user_no", userNo);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var No = reader.GetInt32(0);
                    var name = reader.IsDBNull(1) ? "" : reader.GetString(1);
                    var date = reader.IsDBNull(2) ? DateTime.MinValue : reader.GetDateTime(2);
                    List.Add((No, name, date));
                }
            }
            catch(Exception ex)
            {
                Message = "記録取得エラー" + ex.Message;
                
            }
            return List;
        }
    }

    
}
