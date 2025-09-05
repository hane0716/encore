using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace WebApp.Controllers
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

                string sql = "insert into mst_users(name, create_date, edit_date) values (@name, date_trunc('second', current_timestamp), date_trunc('second', current_timestamp))";
                using var cmd = new NpgsqlCommand(sql, conn);
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

                string sql = "update mst_users set edit_date = date_trunc('second', current_timestamp), delete_date = date_trunc('second', current_timestamp) where name = @name";
                using var cmd = new NpgsqlCommand(sql, conn);
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
