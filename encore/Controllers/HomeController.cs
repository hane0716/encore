using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Text;

namespace encore.Controllers
{
    public class HomeController : Controller
    {
        private readonly string connString = "Host=localhost;Username=postgres;Password=encore;Database=postgres";

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Insert(string name)
        {
            try
            {
                using var conn = new NpgsqlConnection(connString);
                conn.Open();

                StringBuilder sbsql = new StringBuilder();
                sbsql.AppendLine("insert into mst_users(                      ");
                sbsql.AppendLine("     name                                   ");
                sbsql.AppendLine("   , create_date                            ");
                sbsql.AppendLine("   , edit_date                              ");
                sbsql.AppendLine(" ) values (                                 ");
                sbsql.AppendLine("    @name                                   ");
                sbsql.AppendLine("  , date_trunc('second', current_timestamp) ");
                sbsql.AppendLine("  , date_trunc('second', current_timestamp) ");
                sbsql.AppendLine(" )                                          ");

                using var cmd = new NpgsqlCommand(sbsql.ToString(), conn);
                cmd.Parameters.AddWithValue("@name", name);

                cmd.ExecuteNonQuery();

                ViewBag.Message =  name + "　でなまえを登録しました！";
            }
            catch (Exception ex)
            {
                ViewBag.Message = "エラー: " + ex.Message;
            }

            return View("Index");
        }

        public IActionResult Delete(string delname)
        {
            try
            {
                using var conn = new NpgsqlConnection(connString);
                conn.Open();

                StringBuilder sbSql = new StringBuilder();
                sbSql.AppendLine(" update mst_users set edit_date   = date_trunc('second', current_timestamp) ");
                sbSql.AppendLine("                    , delete_date = date_trunc('second', current_timestamp) ");
                sbSql.AppendLine("                where name = @name                                          ");

                using var cmd = new NpgsqlCommand(sbSql.ToString(), conn);
                cmd.Parameters.AddWithValue("@name", delname);

                cmd.ExecuteNonQuery();

                ViewBag.Message = delname + "　のなまえを削除しました。";
            }
            catch (Exception ex)
            {
                ViewBag.Message = "エラー: " + ex.Message;
            }

            return View("Index");
        }

    }
}
