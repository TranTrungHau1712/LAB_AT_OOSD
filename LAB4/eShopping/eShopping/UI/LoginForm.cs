using System;
using System.Drawing;
using System.Windows.Forms;
using eShopping.Services;

namespace eShopping.UI
{
    public class LoginForm : Form
    {
        private readonly TextBox txtUser = Ui.Txt(220);
        private readonly TextBox txtPass = Ui.Txt(220, true);

        public LoginForm()
        {
            Ui.Setup(this, "Đăng nhập", 420, 260);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            var title = new Label { Text = "Đăng nhập", Font = Ui.TitleFont, ForeColor = Ui.Primary, Dock = DockStyle.Top, Height = 50, TextAlign = ContentAlignment.MiddleCenter };

            var form = Ui.Form2Col();
            Ui.AddRow(form, "Tên đăng nhập", txtUser);
            Ui.AddRow(form, "Mật khẩu", txtPass);

            var buttons = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(10, 5, 0, 0) };
            var btnLogin = Ui.Btn("Đăng nhập", OnLogin, 120, true);
            buttons.Controls.Add(btnLogin);
            buttons.Controls.Add(Ui.Btn("Đăng ký mới", OnRegister, 120));
            buttons.Controls.Add(Ui.Btn("Hủy", (s, e) => DialogResult = DialogResult.Cancel, 70));

            // Dock: control add sau nằm trên cùng -> add theo thứ tự ngược
            Controls.Add(buttons);
            Controls.Add(form);
            Controls.Add(title);

            AcceptButton = btnLogin;
        }

        private void OnLogin(object sender, EventArgs e)
        {
            try
            {
                var c = AppServices.Auth.Login(txtUser.Text, txtPass.Text);
                if (c == null) { Ui.Warn(this, "Sai tên đăng nhập hoặc mật khẩu."); return; }
                Session.CurrentCustomer = c;
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex) { Ui.Error(this, "Lỗi đăng nhập: " + ex.Message); }
        }

        private void OnRegister(object sender, EventArgs e)
        {
            using (var f = new RegisterForm())
            {
                if (f.ShowDialog(this) == DialogResult.OK)
                {
                    txtUser.Text = f.RegisteredUsername;
                    txtPass.Clear();
                    txtPass.Focus();
                }
            }
        }
    }
}