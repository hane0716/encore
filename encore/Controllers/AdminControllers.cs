using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;
using System.Text;

namespace encore.Controllers
{
    public class AdminController : Controller
    {
        private readonly string connString = "Host=localhost;Username=postgres;Password=encore;Database=postgres";

        public IActionResult Index()
        {
            GetBandList();
            return View("Index2");
        }

        [HttpPost]
        public IActionResult Insert_band(string band_name)
        {
            try
            {
                GetBandList(band_name);
                if (ViewBag.aliveBandList != null && ViewBag.aliveBandList.Rows.Count > 0)
                {
                        ViewBag.Message = band_name + " は、既に登録されています";
                }
                else
                {
                    InsertBandName(band_name);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = "エラー: " + ex.Message;
            }

            GetBandList();

            return View("Index2");

        }


        public IActionResult Delete_band(string band_name)
        {
            try
            {
                using var conn = new NpgsqlConnection(connString);
                conn.Open();

                StringBuilder sbSql= new StringBuilder();

                sbSql.AppendLine(" update dat_band  set edit_date       = date_trunc('second', current_timestamp) ");
                sbSql.AppendLine("                    , yuukou_end_date = date_trunc('second', current_timestamp) ");
                sbSql.AppendLine("                    , delete_date     = date_trunc('second', current_timestamp) ");
                sbSql.AppendLine("                where band_name       = @band_name                              ");

                using var cmd = new NpgsqlCommand(sbSql.ToString(), conn);
                cmd.Parameters.AddWithValue("@band_name", band_name);

                cmd.ExecuteNonQuery();

                ViewBag.Message = band_name + "　を削除しました。";
            }
            catch (Exception ex)
            {
                ViewBag.Message = "エラー: " + ex.Message;
            }

            GetBandList();

            return View("Index2");
        }


        /// <summary>
        /// 生きている全てのバンド名を取得
        /// </summary>
        private void GetBandList()
        {
            var ds = new DataSet();

            try
            {
                using var conn = new NpgsqlConnection(connString);
                conn.Open();

                StringBuilder sbSql = new StringBuilder();
                sbSql.AppendLine(" select band_name           ");
                sbSql.AppendLine("   from dat_band            ");
                sbSql.AppendLine("  where delete_date is null ");

                using var da = new NpgsqlDataAdapter(sbSql.ToString(), conn);
                da.Fill(ds);

                ViewBag.BandList = ds.Tables[0];
            }
            catch (Exception ex)
            {
                ViewBag.Message = "エラー: " + ex.Message;
            }
        }

        /// <summary>
        /// 生きている任意のバンド名を取得
        /// </summary>
        /// <param name="band_name"></param>
        private void GetBandList(string band_name)
        {
            var ds = new DataSet();

            try
            {
                using var conn = new NpgsqlConnection(connString);
                conn.Open();

                StringBuilder sbSql = new StringBuilder();
                sbSql.AppendLine(" select band_name                       ");
                sbSql.AppendLine("   from dat_band                        ");
                sbSql.AppendLine("  where band_name = '" + band_name + "' ");
                sbSql.AppendLine("    and delete_date is null             ");

                using var da = new NpgsqlDataAdapter(sbSql.ToString(), conn);
                da.Fill(ds);

                ViewBag.aliveBandList = ds.Tables[0];
            }
            catch (Exception ex)
            {
                ViewBag.Message = "エラー: " + ex.Message;
            }
        }

        private void InsertBandName(string bandname)
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
            cmd.Parameters.AddWithValue("@band_name", bandname);
            cmd.ExecuteNonQuery();
         
            ViewBag.Message = bandname + " を登録しました！";
        }


    }
}
