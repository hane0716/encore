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
        public IActionResult Insert()
        {
            try
            {
                using var conn = new NpgsqlConnection(connString);
                conn.Open();

                string sql = "INSERT INTO mst_users (id, name, mail, create_date) VALUES (3, 'ukyo', 'u@gmail.com', current_timestamp)";
                using var cmd = new NpgsqlCommand(sql, conn);
                cmd.ExecuteNonQuery();

                ViewBag.Message = "データを挿入しました！";
            }
            catch (Exception ex)
            {
                ViewBag.Message = "エラー: " + ex.Message;
            }

            return View("Index");
        }
    }
}
