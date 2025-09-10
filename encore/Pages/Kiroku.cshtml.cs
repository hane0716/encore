using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;
using System.Data;
using System.Text;

namespace encore.Pages
{


    public class KirokuModel : BasePageModel
    {
        private readonly string connString = "Host=localhost;Username=postgres;Password=encore;Database=postgres";

        public string Title { get; set; } = "���C�u�L�^���";



        public void OnGet()
        {
            strUserName = GetUserSession("user_name");
            strUserNo = GetUserSession("user_no");
            KaiinMessage = strUserName + "����̃y�[�W�ł�";
            LiveDate = DateTime.Today; // �� ����ŏ����\���������ɂȂ�
        }


        public IActionResult OnPostInsert_Kiroku(string KirokuName,DateTime LiveDate)
        {
            try
            {
                strUserNo = GetUserSession("user_no");
                using var conn = new NpgsqlConnection(connString);
                conn.Open();

                var sbsql = new StringBuilder();
                sbsql.AppendLine(" insert into mst_kiroku(                        ");
                sbsql.AppendLine("        user_no                                 ");
                sbsql.AppendLine("      , live_name                               ");
                sbsql.AppendLine("      , live_date                               ");
                sbsql.AppendLine("      , create_date                             ");
                sbsql.AppendLine("      , edit_date                               ");
                sbsql.AppendLine(" )                                              ");
                sbsql.AppendLine(" values (                                       ");
                sbsql.AppendLine("        @user_no                                ");
                sbsql.AppendLine("      , @live_name                              ");
                sbsql.AppendLine("      , @live_date                              ");
                sbsql.AppendLine("      , date_trunc('second', current_timestamp) ");
                sbsql.AppendLine("      , date_trunc('second', current_timestamp) ");
                sbsql.AppendLine(" )                                              ");

                using var cmd = new NpgsqlCommand(sbsql.ToString(), conn);
                cmd.Parameters.AddWithValue("@user_no", strUserNo);
                cmd.Parameters.AddWithValue("@live_name", KirokuName);
                cmd.Parameters.AddWithValue("@live_date", LiveDate);
                cmd.ExecuteNonQuery();

                Message = $"{LiveDate:yyyy-MM-dd} �J�Â� {KirokuName} ��o�^���܂����I";
            }
            catch (Exception ex)
            {
                Message = "�G���[: " + ex.Message;
            }

            return Page();

        }
    }

    
}
