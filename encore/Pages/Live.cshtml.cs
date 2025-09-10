using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;
using System.Data;
using System.Text;

namespace encore.Pages
{
    public class LiveModel : BasePageModel
    {
        public string Title { get; set; } = "���C�u�L�^���";
        private readonly string connString = "Host=localhost;Username=postgres;Password=encore;Database=postgres";

        [BindProperty]
        public string LiveName { get; set; }

        [BindProperty]
        public DateTime LiveDate { get; set; }

        [BindProperty]
        public string DelLiveName { get; set; }

        public string Message { get; set; }
        public DataTable LiveList { get; set; }

        public string KaiinMessage { get; set; }
        
        public string strUserName { get; set; }



        public void OnGet()
        {
            LiveDate = DateTime.Today;
            strUserName.DefaultIfEmpty();
            strUserName = GetUserSession("user_name");
            if (strUserName != null)
            {
                KaiinMessage = strUserName + "����̃y�[�W�ł�";
            }
            GetLiveList();
        }

        public IActionResult OnPostInsert_live()
        {
            try
            {
                using var conn = new NpgsqlConnection(connString);
                conn.Open();

                var sbsql = new StringBuilder();
                sbsql.AppendLine("insert into mst_live (                         ");
                sbsql.AppendLine("       live_name                               ");
                sbsql.AppendLine("     , live_date                               ");
                sbsql.AppendLine("     , create_date                             ");
                sbsql.AppendLine("     , edit_date                               ");
                sbsql.AppendLine(" )                                             ");
                sbsql.AppendLine(" values (                                      ");
                sbsql.AppendLine("       @live_name                              ");
                sbsql.AppendLine("     , @live_date                              ");
                sbsql.AppendLine("     , date_trunc('second', current_timestamp) ");
                sbsql.AppendLine("     , date_trunc('second', current_timestamp) ");
                sbsql.AppendLine(" )                                             ");

                using var cmd = new NpgsqlCommand(sbsql.ToString(), conn);
                cmd.Parameters.AddWithValue("@live_name", LiveName);
                cmd.Parameters.AddWithValue("@live_date", LiveDate);
                cmd.ExecuteNonQuery();

                Message = $"{LiveDate:yyyy-MM-dd} �J�Â� {LiveName} ��o�^���܂����I";
            }
            catch (Exception ex)
            {
                Message = "�G���[: " + ex.Message;
            }

            GetLiveList();
            return Page();
        }

        public IActionResult OnPostDelete_live()
        {
            try
            {
                using var conn = new NpgsqlConnection(connString);
                conn.Open();

                var sbSql = new StringBuilder();
                sbSql.AppendLine(" update mst_live                                              ");
                sbSql.AppendLine("    set edit_date   = date_trunc('second', current_timestamp) ");
                sbSql.AppendLine("      , delete_date = date_trunc('second', current_timestamp) ");
                sbSql.AppendLine("  where live_name = @name                                     ");

                using var cmd = new NpgsqlCommand(sbSql.ToString(), conn);
                cmd.Parameters.AddWithValue("@name", DelLiveName);
                cmd.ExecuteNonQuery();

                Message = $"{DelLiveName} �̃��C�u�����폜���܂����B";
            }
            catch (Exception ex)
            {
                Message = "�G���[: " + ex.Message;
            }

            GetLiveList();
            return Page();
        }

        private void GetLiveList()
        {
            var ds = new DataSet();
            using var conn = new NpgsqlConnection(connString);
            conn.Open();

            var sbSql = new StringBuilder();
            sbSql.AppendLine("select live_name, live_date          ");
            sbSql.AppendLine("  from mst_live                      ");
            sbSql.AppendLine(" where delete_date is null           ");
            sbSql.AppendLine("    or live_date >= current_timestamp");

            using var da = new NpgsqlDataAdapter(sbSql.ToString(), conn);
            da.Fill(ds);

            LiveList = ds.Tables[0];
        }


        private void GetUserLiveList()
        {
            var ds = new DataSet();
            using var conn = new NpgsqlConnection(connString);
            conn.Open();

            var sbSql = new StringBuilder();
            sbSql.AppendLine("select live_name, live_date from mst_live");
            sbSql.AppendLine("where delete_date is null or live_date >= current_timestamp");

            using var da = new NpgsqlDataAdapter(sbSql.ToString(), conn);
            da.Fill(ds);

            LiveList = ds.Tables[0];
        }

    }
}
