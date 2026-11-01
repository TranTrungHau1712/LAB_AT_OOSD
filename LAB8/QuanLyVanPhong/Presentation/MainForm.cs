using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QuanLyVanPhong.Common;

namespace QuanLyVanPhong.Presentation
{
    /// <summary>2. Form giao diện chính: Sidebar trái, thanh trên (tên hệ thống, tài khoản, đăng xuất), vùng nội dung.</summary>
    public class MainForm : Form
    {
        private readonly Panel content = new Panel();
        private readonly Label lblPage = new Label();
        private readonly Dictionary<string, Button> menuButtons = new Dictionary<string, Button>();
        private Form current;
        public bool LoggedOut;

        private static readonly string[][] Menu = new string[][]
        {
            new[] { "dashboard", "Tổng quan" },
            new[] { "tang", "Quản lý tầng" },
            new[] { "vanphong", "Quản lý văn phòng" },
            new[] { "khach", "Khách hàng & Công ty" },
            new[] { "hopdong", "Hợp đồng thuê" },
            new[] { "giahan", "Gia hạn hợp đồng" },
            new[] { "thanhtoan", "Thanh toán tiền thuê" },
            new[] { "dichvu", "Dịch vụ" },
            new[] { "hoadon", "Hóa đơn dịch vụ" },
            new[] { "nhanvien", "Nhân viên" },
            new[] { "thexe", "Thẻ giữ xe" },
            new[] { "kiemtra", "Kiểm tra văn phòng" },
            new[] { "vipham", "Vi phạm & tiền phạt" },
            new[] { "thongke", "Thống kê & báo cáo" }
        };

        public MainForm()
        {
            Text = "Quản lý cho thuê văn phòng cao ốc";
            Size = new Size(1360, 820);
            MinimumSize = new Size(1100, 700);
            StartPosition = FormStartPosition.CenterScreen;
            Font = Theme.Base;
            BackColor = Theme.Back;

            content.Dock = DockStyle.Fill;
            content.BackColor = Theme.Back;

            // Sidebar
            var side = new FlowLayoutPanel();
            side.Dock = DockStyle.Left;
            side.Width = 240;
            side.BackColor = Theme.PrimaryDark;
            side.FlowDirection = FlowDirection.TopDown;
            side.WrapContents = false;
            side.AutoScroll = true;
            side.Padding = new Padding(0, 12, 0, 0);
            foreach (var m in Menu)
            {
                string key = m[0];
                var b = new Button();
                b.Text = "   " + m[1];
                b.Width = 240;
                b.Height = 44;
                b.Margin = new Padding(0);
                b.FlatStyle = FlatStyle.Flat;
                b.FlatAppearance.BorderSize = 0;
                b.ForeColor = Color.White;
                b.BackColor = Theme.PrimaryDark;
                b.TextAlign = ContentAlignment.MiddleLeft;
                b.Font = new Font("Segoe UI", 10.5F);
                b.Cursor = Cursors.Hand;
                b.Click += delegate { Navigate(key); };
                menuButtons[key] = b;
                side.Controls.Add(b);
            }

            // Thanh trên
            var top = new Panel();
            top.Dock = DockStyle.Top;
            top.Height = 56;
            top.BackColor = Theme.Primary;
            var lblSys = new Label();
            lblSys.Text = "HỆ THỐNG QUẢN LÝ CHO THUÊ VĂN PHÒNG CAO ỐC";
            lblSys.ForeColor = Color.White;
            lblSys.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblSys.AutoSize = true;
            lblSys.Location = new Point(16, 16);
            var right = new FlowLayoutPanel();
            right.Dock = DockStyle.Right;
            right.Width = 360;
            right.FlowDirection = FlowDirection.RightToLeft;
            right.Padding = new Padding(0, 9, 12, 0);
            var btnOut = Theme.Btn("Đăng xuất", Theme.Danger, 100);
            btnOut.Click += delegate
            {
                if (!Theme.Confirm("Bạn có chắc chắn muốn đăng xuất?")) return;
                LoggedOut = true;
                Close();
            };
            var lblUser = new Label();
            lblUser.Text = Session.FullName + " (" + Session.Role + ")";
            lblUser.ForeColor = Color.White;
            lblUser.AutoSize = true;
            lblUser.Margin = new Padding(0, 8, 12, 0);
            right.Controls.Add(btnOut);
            right.Controls.Add(lblUser);
            top.Controls.Add(lblSys);
            top.Controls.Add(right);

            Controls.Add(content);
            Controls.Add(side);
            Controls.Add(top);
            content.BringToFront();

            Load += delegate { Navigate("dashboard"); };
        }

        private Form Create(string key)
        {
            switch (key)
            {
                case "dashboard": return new DashboardForm();
                case "tang": return new TangForm();
                case "vanphong": return new VanPhongForm();
                case "khach": return new KhachHangCongTyForm();
                case "hopdong": return new HopDongForm();
                case "giahan": return new GiaHanForm();
                case "thanhtoan": return new ThanhToanForm();
                case "dichvu": return new DichVuForm();
                case "hoadon": return new HoaDonForm();
                case "nhanvien": return new NhanVienForm();
                case "thexe": return new TheGiuXeForm();
                case "kiemtra": return new KiemTraForm();
                case "vipham": return new ViPhamForm();
                default: return new ThongKeForm();
            }
        }

        private void Navigate(string key)
        {
            try
            {
                foreach (var kv in menuButtons)
                    kv.Value.BackColor = kv.Key == key ? Theme.Accent : Theme.PrimaryDark;
                if (current != null) { content.Controls.Remove(current); current.Dispose(); current = null; }
                Form f = Create(key);
                f.TopLevel = false;
                f.FormBorderStyle = FormBorderStyle.None;
                f.Dock = DockStyle.Fill;
                content.Controls.Add(f);
                f.Show();
                current = f;
            }
            catch (Exception ex)
            {
                Theme.Error("Không mở được chức năng: " + QuanLyVanPhong.DAL.Db.Friendly(ex));
            }
        }
    }
}
