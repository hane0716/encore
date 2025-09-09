using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;
using System.Data;
using System.Text;

namespace encore.Pages
{
    public class AdminModel : BasePageModel
    {
        public string Title { get; set; } = "�Ǘ����";
        // PostgreSQL�ڑ�������i���[�J��DB�j
        private readonly string connString = "Host=localhost;Username=postgres;Password=encore;Database=postgres";

        // �o���h���̓o�^�p�v���p�e�B�i�t�H�[�����͂ƃo�C���h�j
        [BindProperty]
        public string BandName { get; set; }

        // �o���h���̍폜�p�v���p�e�B�i�t�H�[�����͂ƃo�C���h�j
        [BindProperty]
        public string DelBandName { get; set; }

        // �������ʃ��b�Z�[�W�i�o�^�E�폜�E�G���[�Ȃǁj

        // �o���h�ꗗ�\���p�̃f�[�^�e�[�u��
        public DataTable BandList { get; set; }

        // �y�[�W�����\�����iGET���N�G�X�g�j�Ƀo���h�ꗗ���擾
        public void OnGet()
        {
            GetBandList();
        }

        /// <summary>
        /// �o���h����o�^
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostInsert_band()
        {
            try
            {
                // �����o���h�������ɓo�^����Ă��邩�m�F
                var aliveList = GetBandList(BandName);
                if (aliveList != null && aliveList.Rows.Count > 0)
                {
                    Message = $"{BandName} �́A���ɓo�^����Ă��܂�";
                }
                else
                {
                    // �V�K�o�^����
                    InsertBandName(BandName);
                    Message = $"{BandName} ��o�^���܂����I";
                }
            }
            catch (Exception ex)
            {
                // �G���[�������̃��b�Z�[�W
                Message = "�G���[: " + ex.Message;
            }

            // ���͗����N���A
            BandName = string.Empty;

            // �ŐV�̈ꗗ���擾���ĕ\��
            GetBandList();
            return Page();
        }

        /// <summary>
        /// �o���h�����폜
        /// </summary>
        /// <returns></returns>
        public IActionResult OnPostDelete_band()
        {
            try
            {
                using var conn = new NpgsqlConnection(connString);
                conn.Open();

                // �_���폜�idelete_date ��ݒ�j
                var sbSql = new StringBuilder();
                sbSql.AppendLine(" update dat_band  set edit_date       = date_trunc('second', current_timestamp) ");
                sbSql.AppendLine("                    , yuukou_end_date = date_trunc('second', current_timestamp) ");
                sbSql.AppendLine("                    , delete_date     = date_trunc('second', current_timestamp) ");
                sbSql.AppendLine("                where band_name       = @band_name                              ");

                using var cmd = new NpgsqlCommand(sbSql.ToString(), conn);
                cmd.Parameters.AddWithValue("@band_name", DelBandName);
                cmd.ExecuteNonQuery();

                Message = $"{DelBandName} ���폜���܂����B";
            }
            catch (Exception ex)
            {
                Message = "�G���[: " + ex.Message;
            }

            // ���͗����N���A
            DelBandName = string.Empty;

            // �ŐV�̈ꗗ���擾���ĕ\��
            GetBandList();
            return Page();
        }

        /// <summary>
        /// �����Ă���o���h�����擾
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
        /// �C�ӂ̐����Ă���o���h�����擾
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
        /// �o���h����o�^
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
