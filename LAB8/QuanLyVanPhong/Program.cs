using System;
using System.Windows.Forms;
using QuanLyVanPhong.Presentation;

namespace QuanLyVanPhong
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            while (true)
            {
                using (var login = new LoginForm())
                {
                    if (login.ShowDialog() != DialogResult.OK) break;
                }
                var main = new MainForm();
                Application.Run(main);
                bool again = main.LoggedOut;
                main.Dispose();
                if (!again) break;
            }
        }
    }
}
