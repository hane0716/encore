using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;
using System.Text;

namespace encore.Pages
{
    public class EditProfileModel : BasePageModel
    {
        private readonly string connString = "Host=localhost;Username=postgres;Password=encore;Database=postgres";

        public string Title { get; set; } = "プロフィール編集画面";
        [BindProperty]
        public string User_mailadress {  get; set; }
        [BindProperty]
        public DateTime User_birthday { get; set; }

        public void OnGet()
        {
            strUserNo = GetUserSession("user_no");

            using var conn = new NpgsqlConnection(connString);
            conn.Open();

            StringBuilder sbSql = new StringBuilder();

            sbSql.AppendLine(" select mail               "); 
            sbSql.AppendLine("      , birth              ");
            sbSql.AppendLine("   from mst_users          ");
            sbSql.AppendLine("  where user_no = @user_no ");
            
            using var cmd = new NpgsqlCommand(sbSql.ToString(), conn);
            cmd.Parameters.AddWithValue("@user_no", strUserNo);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                User_mailadress = reader.IsDBNull(0) ? "" : reader.GetString(0);
                User_birthday = reader.IsDBNull(1) ? DateTime.Today : reader.GetDateTime(1);
            }
        }


        public IActionResult OnPost()
        {
            strUserNo = GetUserSession("user_no");

            using var conn = new NpgsqlConnection(connString);
            conn.Open();

            StringBuilder sbSql = new StringBuilder();

            sbSql.AppendLine(" update mst_users                                           ");
            sbSql.AppendLine("    set mail = @mail                                        ");
            sbSql.AppendLine("      , birth = @birth                                      ");
            sbSql.AppendLine("      , edit_date = date_trunc('second', current_timestamp) ");
            sbSql.AppendLine("  where user_no = @user_no                                  ");

            using var cmd = new NpgsqlCommand(sbSql.ToString(), conn);
            cmd.Parameters.AddWithValue("@mail", User_mailadress ?? "");
            cmd.Parameters.AddWithValue("@birth", User_birthday);
            cmd.Parameters.AddWithValue("@user_no", strUserNo);

            cmd.ExecuteNonQuery();

            Message = "編集が完了しました！";
            return Page();
        }
    }
}
