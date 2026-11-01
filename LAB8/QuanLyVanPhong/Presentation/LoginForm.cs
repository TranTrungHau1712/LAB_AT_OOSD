using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyVanPhong.BLL;
using QuanLyVanPhong.DAL;

namespace QuanLyVanPhong.Presentation
{
    /// <summary>1. Form đăng nhập.</summary>
    public class LoginForm : Form
    {
        private TextBox txtUser, txtPass;

        public LoginForm()
        {
            Text = "Đăng nhập - Quản lý cho thuê văn phòng cao ốc";
            ClientSize = new Size(760, 420);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Font = Theme.Base;
            BackColor = Color.White;

            var left = new Panel();
            left.Dock = DockStyle.Left;
            left.Width = 300;
            left.BackColor = Theme.Primary;
            var brand = new Label();
            brand.Text = "QUẢN LÝ\nCHO THUÊ\nVĂN PHÒNG\nCAO ỐC";
            brand.ForeColor = Color.White;
            brand.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            brand.AutoSize = true;
            brand.Location = new Point(32, 110);
            var sub = new Label();
            sub.Text = "Phần mềm quản lý hợp đồng,\nthanh toán và dịch vụ";
            sub.ForeColor = Color.FromArgb(190, 205, 225);
            sub.AutoSize = true;
            sub.Location = new Point(34, 290);
            left.Controls.Add(brand);
            left.Controls.Add(sub);

            var right = new Panel();
            right.Dock = DockStyle.Fill;
            right.Padding = new Padding(40);

            var title = new Label();
            title.Text = "ĐĂNG NHẬP";
            title.Font = Theme.Title;
            title.ForeColor = Theme.Primary;
            title.AutoSize = true;
            title.Location = new Point(40, 50);

            var l1 = new Label(); l1.Text = "Tên đăng nhập"; l1.AutoSize = true; l1.Location = new Point(40, 120);
            txtUser = new TextBox(); txtUser.Location = new Point(40, 146); txtUser.Width = 360; txtUser.Font = new Font("Segoe UI", 11F);
            var l2 = new Label(); l2.Text = "Mật khẩu"; l2.AutoSize = true; l2.Location = new Point(40, 192);
            txtPass = new TextBox(); txtPass.Location = new Point(40, 218); txtPass.Width = 360; txtPass.Font = new Font("Segoe UI", 11F);
            txtPass.UseSystemPasswordChar = true;

            var btn = Theme.Btn("Đăng nhập", Theme.Primary, 170);
            btn.Location = new Point(40, 280);
            btn.Click += delegate { DoLogin(); };
            var btnExit = Theme.Btn("Thoát", Color.Gray, 170);
            btnExit.Location = new Point(230, 280);
            btnExit.Click += delegate { DialogResult = DialogResult.Cancel; Close(); };
            var hint = new Label();
            hint.Text = "Tài khoản mẫu: admin / admin123";
            hint.ForeColor = Color.Gray;
            hint.AutoSize = true;
            hint.Location = new Point(40, 340);

            right.Controls.AddRange(new Control[] { title, l1, txtUser, l2, txtPass, btn, btnExit, hint });
            Controls.Add(right);
            Controls.Add(left);
            AcceptButton = btn;
        }

        private void DoLogin()
        {
            try
            {
                AuthBll.Login(txtUser.Text, txtPass.Text);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                Theme.Error(Db.Friendly(ex));
                txtPass.Clear();
                txtPass.Focus();
            }
        }
    }
}
