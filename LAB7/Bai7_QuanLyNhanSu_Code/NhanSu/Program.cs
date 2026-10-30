using System;
using System.Windows.Forms;

namespace NhanSu
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                Db.Scalar("SELECT 1");   // kiểm tra kết nối trước khi mở giao diện
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không kết nối được CSDL QuanLyNhanSu.\nKiểm tra lại connectionString trong App.config và đã chạy file SQL chưa.\n\n" + ex.Message,
                    "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Application.Run(new FrmMain());
        }
    }
}
