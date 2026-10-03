using System;
using System.Drawing;
using System.Windows.Forms;
using eShopping.Models;
using eShopping.Services;

namespace eShopping.UI
{
    public class RegisterForm : Form
    {
        private readonly TextBox txtName = Ui.Txt(300), txtIdNo = Ui.Txt(300), txtAddr = Ui.Txt(300),
            txtPhone = Ui.Txt(300), txtUser = Ui.Txt(300), txtPass = Ui.Txt(300, true),
            txtPass2 = Ui.Txt(300, true), txtEmail = Ui.Txt(300);
        private readonly DateTimePicker dtpBirth = new DateTimePicker
        {
            Format = DateTimePickerFormat.Custom,
            CustomFormat = "dd/MM/yyyy",
            Width = 140,
            Font = Ui.BaseFont,
            Value = new DateTime(2000, 1, 1),
            MaxDate = DateTime.Today
        };

        public string RegisteredUsername { get; private set; }

        public RegisterForm()
        {
            Ui.Setup(this, "Đăng ký tài khoản khách hàng", 520, 520);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            var title = new Label { Text = "Đăng ký tài khoản", Font = Ui.TitleFont, ForeColor = Ui.Primary, Dock = DockStyle.Top, Height = 50, TextAlign = ContentAlignment.MiddleCenter };

            var form = Ui.Form2Col();
            Ui.AddRow(form, "Họ tên (*)", txtName);
            Ui.AddRow(form, "Ngày sinh (*)", dtpBirth);
            Ui.AddRow(form, "CMND/Passport (*)", txtIdNo);
            Ui.AddRow(form, "Địa chỉ (*)", txtAddr);
            Ui.AddRow(form, "Điện thoại (*)", txtPhone);
            Ui.AddRow(form, "Tên đăng nhập (*)", txtUser);
            Ui.AddRow(form, "Mật khẩu (*)", txtPass);
            Ui.AddRow(form, "Nhập lại mật khẩu (*)", txtPass2);
            Ui.AddRow(form, "Email", txtEmail);

            var hint = new Label { Text = "(*) bắt buộc. Email dùng để nhận xác nhận mỗi khi mua hàng.", AutoSize = true, ForeColor = Color.Gray, Dock = DockStyle.Top, Padding = new Padding(12, 0, 0, 0) };

            var buttons = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(10, 5, 0, 0) };
            var btnOk = Ui.Btn("Đăng ký", OnRegister, 120, true);
            buttons.Controls.Add(btnOk);
            buttons.Controls.Add(Ui.Btn("Hủy", (s, e) => DialogResult = DialogResult.Cancel, 80));

            Controls.Add(buttons);
            Controls.Add(hint);
            Controls.Add(form);
            Controls.Add(title);
            AcceptButton = btnOk;
        }

        private void OnRegister(object sender, EventArgs e)
        {
            var c = new Customer
            {
                FullName = txtName.Text.Trim(),
                BirthDate = dtpBirth.Value.Date,
                IdNumber = txtIdNo.Text.Trim(),
                Address = txtAddr.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                Username = txtUser.Text.Trim(),
                Email = txtEmail.Text.Trim()
            };
            try
            {
                string err = AppServices.Auth.Register(c, txtPass.Text, txtPass2.Text);
                if (err != null) { Ui.Warn(this, err); return; }
                RegisteredUsername = c.Username;
                Ui.Info(this, "Đăng ký thành công! Bạn có thể đăng nhập.");
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex) { Ui.Error(this, "Lỗi đăng ký: " + ex.Message); }
        }
    }
}