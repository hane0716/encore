using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;
using System.Text;

namespace encore.Pages
{
    public class EditKirokuModel : BasePageModel
    {
        private readonly string connString = "Host=localhost;Username=postgres;Password=encore;Database=postgres";

        [BindProperty]
        public string KirokuName { get; set; }

        [BindProperty]
        public DateTime LiveDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public int KirokuNo { get; set; }

        public void OnGet()
        {
            strUserNo = GetUserSession("user_no");

            using var conn = new NpgsqlConnection(connString);
            conn.Open();

            StringBuilder sbSql = new StringBuilder();

            sbSql.AppendLine(" SELECT live_name              ");
            sbSql.AppendLine("      , live_date              ");
            sbSql.AppendLine("   FROM mst_kiroku             ");
            sbSql.AppendLine("  WHERE kiroku_no = @kiroku_no ");
            sbSql.AppendLine("    AND user_no = @user_no     ");
            sbSql.AppendLine("    AND delete_date IS NULL    ");

            using var cmd = new NpgsqlCommand(sbSql.ToString(), conn);
            cmd.Parameters.AddWithValue("@kiroku_no", KirokuNo);
            cmd.Parameters.AddWithValue("@user_no", strUserNo);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                KirokuName = reader.GetString(0);
                LiveDate = reader.GetDateTime(1);
            }
        }

        public IActionResult OnPost()
        {
            strUserNo = GetUserSession("user_no");

            using var conn = new NpgsqlConnection(connString);
            conn.Open();

            StringBuilder sbSql = new StringBuilder();

            sbSql.AppendLine(" UPDATE mst_kiroku                    ");
            sbSql.AppendLine("    SET live_name = @name             ");
            sbSql.AppendLine("      , live_date = @date             ");
            sbSql.AppendLine("      , edit_date = current_timestamp ");
            sbSql.AppendLine("  WHERE kiroku_no = @no               ");
            sbSql.AppendLine("    AND user_no = @user_no            ");
            
            using var cmd = new NpgsqlCommand(sbSql.ToString(), conn);
            cmd.Parameters.AddWithValue("@name", KirokuName);
            cmd.Parameters.AddWithValue("@date", LiveDate);
            cmd.Parameters.AddWithValue("@no", KirokuNo);
            cmd.Parameters.AddWithValue("@user_no", strUserNo);

            cmd.ExecuteNonQuery();

            Message = "ï“èWÇ™äÆóπÇµÇ‹ÇµÇΩÅI";
            return RedirectToPage("Kiroku");
        }
    }
}
