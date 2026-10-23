using System;
using System.Windows.Forms;
using QuanLyBenhAn.Data;
using QuanLyBenhAn.Forms;

namespace QuanLyBenhAn
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string loi = Db.TestConnection();
            if (loi != null)
            {
                MessageBox.Show(
                    "Không kết nối được CSDL QuanLyBenhAn.\n\n" + loi +
                    "\n\nHãy chạy file SQL/01_QuanLyBenhAn.sql trong SSMS và sửa chuỗi kết nối trong Data/Db.cs (biến ConnectionString).",
                    "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Application.Run(new MainForm());
        }
    }
}
