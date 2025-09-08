using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;
using System.Data;
using System.Text;

namespace encore.Pages
{
    public class AdminModel : PageModel
    {
        private readonly string connString = "Host=localhost;Username=postgres;Password=encore;Database=postgres";

        [BindProperty]
        public string BandName { get; set; }

        [BindProperty]
        public string DelBandName { get; set; }

        public string Message { get; set; }
        public DataTable BandList { get; set; }

        public void OnGet()
        {
            GetBandList();
        }

        public IActionResult OnPostInsert_band()
        {
            try
            {
                var aliveList = GetBandList(BandName);
                if (aliveList != null && aliveList.Rows.Count > 0)
                {
                    Message = $"{BandName} ÇÕÅAä˘Ç…ìoò^Ç≥ÇÍÇƒÇ¢Ç‹Ç∑";
                }
                else
                {
                    InsertBandName(BandName);
                }
            }
            catch (Exception ex)
            {
                Message = "ÉGÉâÅ[: " + ex.Message;
            }

            GetBandList();
            return Page();
        }

        public IActionResult OnPostDelete_band()
        {
            try
            {
                using var conn = new NpgsqlConnection(connString);
                conn.Open();

                var sbSql = new StringBuilder();
                sbSql.AppendLine(" update dat_band  set edit_date       = date_trunc('second', current_timestamp) ");
                sbSql.AppendLine("                    , yuukou_end_date = date_trunc('second', current_timestamp) ");
                sbSql.AppendLine("                    , delete_date     = date_trunc('second', current_timestamp) ");
                sbSql.AppendLine("                where band_name       = @band_name                              ");

                using var cmd = new NpgsqlCommand(sbSql.ToString(), conn);
                cmd.Parameters.AddWithValue("@band_name", DelBandName);
                cmd.ExecuteNonQuery();

                Message = $"{DelBandName} ÇçÌèúÇµÇ‹ÇµÇΩÅB";
            }
            catch (Exception ex)
            {
                Message = "ÉGÉâÅ[: " + ex.Message;
            }

            GetBandList();
            return Page();
        }

        private void GetBandList()
        {
            var ds = new DataSet();
            using var conn = new NpgsqlConnection(connString);
            conn.Open();

            var sbSql = new StringBuilder();
            sbSql.AppendLine(" select band_name           ");
            sbSql.AppendLine("   from dat_band            ");
            sbSql.AppendLine("  where delete_date is null ");

            using var da = new NpgsqlDataAdapter(sbSql.ToString(), conn);
            da.Fill(ds);

            BandList = ds.Tables[0];
        }

        private DataTable GetBandList(string band_name)
        {
            var ds = new DataSet();
            using var conn = new NpgsqlConnection(connString);
            conn.Open();

            var sbSql = new StringBuilder();
            sbSql.AppendLine(" select band_name                       ");
            sbSql.AppendLine("   from dat_band                        ");
            sbSql.AppendLine("  where band_name = @band_name          ");
            sbSql.AppendLine("    and delete_date is null             ");

            using var cmd = new NpgsqlCommand(sbSql.ToString(), conn);
            cmd.Parameters.AddWithValue("@band_name", band_name);

            using var da = new NpgsqlDataAdapter(cmd);
            da.Fill(ds);

            return ds.Tables[0];
        }

        private void InsertBandName(string bandname)
        {
            using var conn = new NpgsqlConnection(connString);
            conn.Open();

            var sbsql = new StringBuilder();
            sbsql.AppendLine(" insert into dat_band(                      ");
            sbsql.AppendLine("     band_name                              ");
            sbsql.AppendLine("   , yuukou_start_date                      ");
            sbsql.AppendLine("   , create_date                            ");
            sbsql.AppendLine("   , edit_date                              ");
            sbsql.AppendLine(" ) values (                                 ");
            sbsql.AppendLine("    @band_name                              ");
            sbsql.AppendLine("  , date_trunc('second', current_timestamp) ");
            sbsql.AppendLine("  , date_trunc('second', current_timestamp) ");
            sbsql.AppendLine("  , date_trunc('second', current_timestamp) ");
            sbsql.AppendLine(" )                                          ");

            using var cmd = new NpgsqlCommand(sbsql.ToString(), conn);
            cmd.Parameters.AddWithValue("@band_name", bandname);
            cmd.ExecuteNonQuery();

            Message = $"{bandname} Çìoò^ÇµÇ‹ÇµÇΩÅI";
        }
    }
}
