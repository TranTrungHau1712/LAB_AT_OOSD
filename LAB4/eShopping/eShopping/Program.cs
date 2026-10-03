using System;
using System.Windows.Forms;
using eShopping.Data;
using eShopping.UI;

namespace eShopping
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                using (var c = Db.Open()) { }   // kiểm tra kết nối CSDL ngay khi khởi động
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không kết nối được CSDL eShoppingDB.\n" +
                                "Hãy chạy database\\eShopping.sql và sửa connectionString trong App.config.\n\n" + ex.Message,
                                "e-Shopping", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Application.Run(new MainForm());
        }
    }
}