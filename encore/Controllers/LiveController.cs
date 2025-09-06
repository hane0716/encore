using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Text;

namespace encore.Controllers
{
    public class LiveController : Controller
    {
        private readonly string connString = "Host=localhost;Username=postgres;Password=encore;Database=postgres";

        public IActionResult Index()
        {
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

            return View("Index1");

        }


        public IActionResult Delete_live(string del_live_name)
        {
            try
            {
                using var conn = new NpgsqlConnection(connString);
                conn.Open();

                string sql = "update mst_live set edit_date = date_trunc('second', current_timestamp), delete_date = date_trunc('second', current_timestamp) where live_name = @name";
                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@name", del_live_name);

                cmd.ExecuteNonQuery();

                ViewBag.Message = del_live_name + "　のなまえを削除しました。";
            }
            catch (Exception ex)
            {
                ViewBag.Message = "エラー: " + ex.Message;
            }

            return View("Index1");
        }




    }
}
