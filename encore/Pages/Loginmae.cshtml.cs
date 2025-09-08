using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;
using System.Text;

namespace encore.Pages
{
    public class LoginmaeModel : PageModel
    {
        private readonly string connString = "Host=localhost;Username=postgres;Password=encore;Database=postgres";

        [BindProperty]
        public string Name { get; set; }

        [BindProperty]
        public string DelName { get; set; }

        public string Message { get; set; }

        public void OnGet() { }

        public void OnPostInsert()
        {
            try
            {
                using var conn = new NpgsqlConnection(connString);
                conn.Open();

                var sql = new StringBuilder();
                sql.AppendLine("insert into mst_users(name, create_date, edit_date)");
                sql.AppendLine("values (@name, date_trunc('second', current_timestamp), date_trunc('second', current_timestamp))");

                using var cmd = new NpgsqlCommand(sql.ToString(), conn);
                cmd.Parameters.AddWithValue("@name", Name);
                cmd.ExecuteNonQuery();

                Message = $"{Name}　でなまえを登録しました！";
            }
            catch (Exception ex)
            {
                Message = "エラー: " + ex.Message;
            }
        }

        public void OnPostDelete()
        {
            try
            {
                using var conn = new NpgsqlConnection(connString);
                conn.Open();

                var sql = new StringBuilder();
                sql.AppendLine("update mst_users set edit_date = date_trunc('second', current_timestamp),");
                sql.AppendLine("delete_date = date_trunc('second', current_timestamp) where name = @name");

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
