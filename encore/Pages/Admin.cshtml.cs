using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;
using System.Data;
using System.Text;

namespace encore.Pages
{
    public class AdminModel : BasePageModel
    {
        public string Title { get; set; } = "管理画面";
        // PostgreSQL接続文字列（ローカルDB）
        private readonly string connString = "Host=localhost;Username=postgres;Password=encore;Database=postgres";

        // バンド名の登録用プロパティ（フォーム入力とバインド）
        [BindProperty]
        public string BandName { get; set; }

        // バンド名の削除用プロパティ（フォーム入力とバインド）
        [BindProperty]
        public string DelBandName { get; set; }

        // 処理結果メッセージ（登録・削除・エラーなど）

        // バンド一覧表示用のデータテーブル
        public DataTable BandList { get; set; }

        // ページ初期表示時（GETリクエスト）にバンド一覧を取得
        public void OnGet()
        {
            GetBandList();
        }

        /// <summary>
        /// バンド名を登録
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostInsert_band()
        {
            try
            {
                // 同じバンド名が既に登録されているか確認
                var aliveList = GetBandList(BandName);
                if (aliveList != null && aliveList.Rows.Count > 0)
                {
                    Message = $"{BandName} は、既に登録されています";
                }
                else
                {
                    // 新規登録処理
                    InsertBandName(BandName);
                    Message = $"{BandName} を登録しました！";
                }
            }
            catch (Exception ex)
            {
                // エラー発生時のメッセージ
                Message = "エラー: " + ex.Message;
            }

            // 入力欄をクリア
            BandName = string.Empty;

            // 最新の一覧を取得して表示
            GetBandList();
            return Page();
        }

        /// <summary>
        /// バンド名を削除
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostDelete_band()
        {
            try
            {
                using var conn = new NpgsqlConnection(connString);
                conn.Open();

                // 論理削除（delete_date を設定）
                var sbSql = new StringBuilder();
                sbSql.AppendLine(" update dat_band  set edit_date       = date_trunc('second', current_timestamp) ");
                sbSql.AppendLine("                    , yuukou_end_date = date_trunc('second', current_timestamp) ");
                sbSql.AppendLine("                    , delete_date     = date_trunc('second', current_timestamp) ");
                sbSql.AppendLine("                where band_name       = @band_name                              ");

                using var cmd = new NpgsqlCommand(sbSql.ToString(), conn);
                cmd.Parameters.AddWithValue("@band_name", DelBandName);
                cmd.ExecuteNonQuery();

                Message = $"{DelBandName} を削除しました。";
            }
            catch (Exception ex)
            {
                Message = "エラー: " + ex.Message;
            }

            // 入力欄をクリア
            DelBandName = string.Empty;

            // 最新の一覧を取得して表示
            GetBandList();
            return Page();
        }

        /// <summary>
        /// 生きているバンド名を取得
        /// </summary>
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

        /// <summary>
        /// 任意の生きているバンド名を取得
        /// </summary>
        /// <param name="band_name"></param>
        /// <returns></returns>
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

        /// <summary>
        /// バンド名を登録
        /// </summary>
        /// <param name="bandname"></param>
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
        }
    }
}
