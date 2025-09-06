using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Text;

namespace encore.Controllers
{
    public class AdminController : Controller
    {
        private readonly string connString = "Host=localhost;Username=postgres;Password=encore;Database=postgres";

        public IActionResult Index()
        {
            return View("Index2");
        }

        [HttpPost]
        public IActionResult Insert_band(string band_name)
        {
            try
            {
                using var conn = new NpgsqlConnection(connString);
                conn.Open();

                StringBuilder sbsql = new StringBuilder();
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
                cmd.Parameters.AddWithValue("@band_name", band_name);

                cmd.ExecuteNonQuery();

                ViewBag.Message = band_name + " を登録しました！";
            }
            catch (Exception ex)
            {
                ViewBag.Message = "エラー: " + ex.Message;
            }

            return View("Index2");

        }


        public IActionResult Delete_band(string del_band_name)
        {
            try
            {
                using var conn = new NpgsqlConnection(connString);
                conn.Open();

                StringBuilder sbSql= new StringBuilder();

                sbSql.AppendLine(" update dat_band                                                 ");
                sbSql.AppendLine("    set edit_date = date_trunc('second', current_timestamp)      ");
                sbSql.AppendLine("   , yuukou_start_date = date_trunc('second', current_timestamp) ");
                sbSql.AppendLine("   , delete_date = date_trunc('second', current_timestamp)       ");
                sbSql.AppendLine("  where band_name = @band_name                                   ");

                using var cmd = new NpgsqlCommand(sbSql.ToString(), conn);
                cmd.Parameters.AddWithValue("@band_name", del_band_name);

                cmd.ExecuteNonQuery();

                ViewBag.Message = del_band_name + "　のなまえを削除しました。";
            }
            catch (Exception ex)
            {
                ViewBag.Message = "エラー: " + ex.Message;
            }

            return View("Index2");
        }




    }
}
