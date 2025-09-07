using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Text;
using System.Data;

namespace encore.Controllers
{
    public class LiveController : Controller
    {
        private readonly string connString = "Host=localhost;Username=postgres;Password=encore;Database=postgres";

        public IActionResult Index()
        {
            GetLiveList();
            return View("Index1");
        }

        [HttpPost]
        public IActionResult Insert_live(string live_name,DateOnly live_date) {
            try
            {
                using var conn = new NpgsqlConnection(connString);
                conn.Open();

                StringBuilder sbsql = new StringBuilder();
                sbsql.AppendLine("insert into mst_live(                       ");
                sbsql.AppendLine("     live_name                              ");
                sbsql.AppendLine("   , live_date                              ");
                sbsql.AppendLine("   , create_date                            ");
                sbsql.AppendLine("   , edit_date                              ");
                sbsql.AppendLine(" ) values (                                 ");
                sbsql.AppendLine("    @live_name                              ");
                sbsql.AppendLine("  , @live_date                              ");
                sbsql.AppendLine("  , date_trunc('second', current_timestamp) ");
                sbsql.AppendLine("  , date_trunc('second', current_timestamp) ");
                sbsql.AppendLine(" )                                          ");
                using var cmd = new NpgsqlCommand(sbsql.ToString(), conn);
                cmd.Parameters.AddWithValue("@live_name", live_name);
                cmd.Parameters.AddWithValue("@live_date", live_date);

                cmd.ExecuteNonQuery();

                ViewBag.Message = live_date + " 開催の " + live_name + " を登録しました！";
            }
            catch (Exception ex)
            {
                ViewBag.Message = "エラー: " + ex.Message;
            }

            GetLiveList();

            return View("Index1");

        }


        public IActionResult Delete_live(string del_live_name)
        {
            try
            {
                using var conn = new NpgsqlConnection(connString);
                conn.Open();

                StringBuilder sbSql = new StringBuilder();
                sbSql.AppendLine(" update mst_live set edit_date   = date_trunc('second', current_timestamp) ");
                sbSql.AppendLine("                   , delete_date = date_trunc('second', current_timestamp) ");
                sbSql.AppendLine("               where live_name   = @name                                     ");

                using var cmd = new NpgsqlCommand(sbSql.ToString(), conn);
                cmd.Parameters.AddWithValue("@name", del_live_name);

                cmd.ExecuteNonQuery();

                ViewBag.Message = del_live_name + "　のなまえを削除しました。";
            }
            catch (Exception ex)
            {
                ViewBag.Message = "エラー: " + ex.Message;
            }

            GetLiveList();

            return View("Index1");
        }


        private void GetLiveList()
        {
            var ds = new DataSet();

            try
            {
                using var conn = new NpgsqlConnection(connString);
                conn.Open();

                StringBuilder sbSql = new StringBuilder();
                sbSql.AppendLine(" select live_name                      ");
                sbSql.AppendLine("      , live_date                      ");
                sbSql.AppendLine("   from mst_live                       ");
                sbSql.AppendLine("  where delete_date is null            ");
                sbSql.AppendLine("     or live_date >= current_timestamp ");

                using var da = new NpgsqlDataAdapter(sbSql.ToString(), conn);
                da.Fill(ds);

                ViewBag.LiveList = ds.Tables[0];
            }
            catch (Exception ex)
            {
                ViewBag.Message = "エラー: " + ex.Message;
            }
        }


    }
}
