using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;
using System.Data;
using System.Text;

namespace encore.Pages
{
    public class BasePageModel : PageModel
    {
        public string KaiinMessage { get; set; }

        public string strUserName { get; set; }

        public string strUserNo { get; set; }

        public string KirokuName { get; set; }

        public DateTime LiveDate { get; set; }

        public string Message { get; set; }
        public string WelcomeMessage { get; set; }

        public string Name { get; set; }

        public string DelName { get; set; }

        //public DataSet UserNameList {  get; set; }

        private readonly string connString = "Host=localhost;Username=postgres;Password=encore;Database=postgres";

        /// <summary>
        /// ユーザー情報をセッションで保存する
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        protected void SetUserSession(string key, string value)
        {
            HttpContext.Session.SetString(key, value);
        }

        /// <summary>
        /// ユーザー情報をセッションから取得する
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected string GetUserSession(string key)
        {
            return HttpContext.Session.GetString(key);
        }

        /// <summary>
        /// ユーザー情報のセッションから key に結び付いた値を削除
        /// </summary>
        /// <param name="key"></param>
        protected void ClearUserSession(string key)
        {
            HttpContext.Session.Remove(key);
        }


        /// <summary>
        /// なまえから user_no を取得する
        /// </summary>
        /// <param name="user_name"></param>
        /// <returns></returns>
        public string NameToNo(string user_name)
        {
            try
            {
                using var conn = new NpgsqlConnection(connString);
                conn.Open();

                StringBuilder sbSql = new StringBuilder();

                sbSql.AppendLine(" select user_no                ");
                sbSql.AppendLine("   from mst_users              ");
                sbSql.AppendLine("  where user_name = @user_name ");

                using var cmd = new NpgsqlCommand(sbSql.ToString(), conn);
                cmd.Parameters.AddWithValue("@user_name", user_name);
                
                string result = cmd.ExecuteScalar().ToString();

                return result;
            }
            catch (Exception ex)
            {
                Message = "エラー: " + ex.Message;
                return ex.Message;
            }
        }

        /// <summary>
        /// 引数のなまえでユーザー登録する
        /// </summary>
        /// <param name="Name"></param>
        public void InsertUsers(string Name)
        {
            try
            {
                using var conn = new NpgsqlConnection(connString);
                conn.Open();

                string User_no1 = GetMaxUserNoPlus1();
                StringBuilder sbSql = new StringBuilder();

                sbSql.AppendLine(" insert into mst_users (                        ");
                sbSql.AppendLine("        user_no                                 ");
                sbSql.AppendLine("      , user_name                               ");
                sbSql.AppendLine("      , create_date                             ");
                sbSql.AppendLine("      , edit_date                               ");
                sbSql.AppendLine(" )                                              ");
                sbSql.AppendLine(" values (                                       ");
                sbSql.AppendLine("        @user_no1                               ");
                sbSql.AppendLine("      , @user_name                              ");
                sbSql.AppendLine("      , date_trunc('second', current_timestamp) ");
                sbSql.AppendLine("      , date_trunc('second', current_timestamp) ");
                sbSql.AppendLine(" )                                              ");
                
                using var cmd = new NpgsqlCommand(sbSql.ToString(), conn);
                cmd.Parameters.AddWithValue("@user_no1", User_no1);
                cmd.Parameters.AddWithValue("@user_name", Name);
                cmd.ExecuteNonQuery();
            }
            catch(Exception ex) 
            {
                throw new Exception("ユーザー登録に失敗しました", ex);
            }
        }

        /// <summary>
        /// user_no + 1 を取得する
        /// </summary>
        /// <returns></returns>
        public string GetMaxUserNoPlus1()
        {
            try
            {
                int user_no1 = 1;
                //TryParse()は文字列を安全にintに変換する方法。
                //変換に成功すればtrue を返してmaxNoにその値が入る。
                //変換に失敗すればfalseを返してmaxNoは使われない()。
                if (int.TryParse(GetMaxUserNo(), out var maxNo))
                {
                    user_no1 = maxNo + 1;
                }

                return user_no1.ToString();
            }
            catch( Exception ex)
            {
                Message = "エラー: " + ex.Message;

                return ex.Message;
            }
        }

        /// <summary>
        /// 最大の user_no を取得する
        /// </summary>
        /// <returns></returns>
        public string GetMaxUserNo()
        {
            using var conn = new NpgsqlConnection(connString);
            conn.Open();

            StringBuilder sbSql = new StringBuilder();

            sbSql.AppendLine(" select max(cast(user_no as numeric)) ");
            sbSql.AppendLine("   from mst_users                     ");

            using var cmd = new NpgsqlCommand(sbSql.ToString(), conn);

            var result = cmd.ExecuteScalar();
            //三項演算子（条件 ? 真の値 : 偽の値)
            //resultがDBNullの場合はnullを返す
            return result != DBNull.Value ? result?.ToString() : null;
        }

        /// <summary>
        /// user_noが生きていればtrueを返す
        /// 会員情報が生きているかチェックする
        /// </summary>
        /// <param name="user_no"></param>
        /// <returns></returns>
        public bool CheckUser(string user_no)
        {
            try
            {
                var userTable = GetUserNoList(user_no);

                // 行が1つ以上あれば存在する → true
                if (userTable != null && userTable.Rows.Count > 0)
                {
                    return true;
                }

                // 行がなければ存在しない → false
                return false;
            }
            catch (Exception ex)
            {
                // 例外が出たら false（ログ出力なども検討してね）
                return false;
            }
        }




        /// <summary>
        /// 生きているユーザーnoを取得する
        /// </summary>
        /// <param name="user_no"></param>
        /// <returns></returns>
        private DataTable GetUserNoList(string user_no)
        {
            var ds = new DataSet();
            using var conn = new NpgsqlConnection(connString);
            conn.Open();

            var sbSql = new StringBuilder();
            sbSql.AppendLine(" select user_no             ");
            sbSql.AppendLine("   from mst_users           ");
            sbSql.AppendLine("  where user_no = @user_no  ");
            sbSql.AppendLine("    and delete_date is null ");

            using var cmd = new NpgsqlCommand(sbSql.ToString(), conn);
            cmd.Parameters.AddWithValue("@user_no", user_no);

            using var da = new NpgsqlDataAdapter(cmd);
            da.Fill(ds);

            return ds.Tables[0];
        }




    }
}
